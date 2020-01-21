using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.ResetPageAnswers
{
    public class ResetPageAnswersHandler : SetAnswersBase, IRequestHandler<ResetPageAnswersRequest, HandlerResponse<ResetPageAnswersResponse>>
    {
        public ResetPageAnswersHandler(QnaDataContext dataContext) : base(dataContext)
        {
        }

        public async Task<HandlerResponse<ResetPageAnswersResponse>> Handle(ResetPageAnswersRequest request, CancellationToken cancellationToken)
        {
            var validationErrorResponse = ValidateRequest(request);

            if (validationErrorResponse != null)
            {
                return validationErrorResponse;
            }

            ResetPageAnswers(request);
            UpdateApplicationData(request);

            var nextAction = GetNextActionForPage(request.SectionId, request.PageId);
            var checkboxListAllNexts = GetCheckboxListMatchingNextActionsForPage(request.SectionId, request.PageId);

            SetStatusOfNextPagesBasedOnDeemedNextActions(request.SectionId, request.PageId, nextAction, checkboxListAllNexts);

            return new HandlerResponse<ResetPageAnswersResponse>(new ResetPageAnswersResponse(true));
        }

        private HandlerResponse<ResetPageAnswersResponse> ValidateRequest(ResetPageAnswersRequest request)
        {
            var section = _dataContext.ApplicationSections.AsNoTracking().SingleOrDefault(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId);
            var page = section?.QnAData?.Pages.SingleOrDefault(p => p.PageId == request.PageId);

            if (page is null)
            {
                return new HandlerResponse<ResetPageAnswersResponse>(success: false, message: "Cannot find requested page.");
            }
            else if (page.Questions.Count > 0)
            {
                if (page.Questions.All(q => "FileUpload".Equals(q.Input?.Type, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return new HandlerResponse<ResetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for FileUpload questions. Use Upload / DeleteFile instead.");
                }
                else if (page.Questions.Any(q => "FileUpload".Equals(q.Input?.Type, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return new HandlerResponse<ResetPageAnswersResponse>(success: false, message: "Pages cannot contain a mixture of FileUploads and other Question Types.");
                }
            }

            return null;
        }

        private void ResetPageAnswers(ResetPageAnswersRequest request)
        {
            var section = _dataContext.ApplicationSections.SingleOrDefault(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId);

            if (section != null)
            {
                // Have to force QnAData a new object and reassign for Entity Framework to pick up changes
                var qnaData = new QnAData(section.QnAData);
                var page = qnaData?.Pages.SingleOrDefault(p => p.PageId == request.PageId);

                if (page != null)
                {
                    page.PageOfAnswers = new List<PageOfAnswers>();

                    page.Complete = false;
                    MarkPageFeedbackAsComplete(page); // As the answer has been 'changed', feedback can be deemed as completed

                    // Assign QnAData back so Entity Framework will pick up changes
                    section.QnAData = qnaData;
                    _dataContext.SaveChanges();
                }
            }
        }

        private void UpdateApplicationData(ResetPageAnswersRequest request)
        {
            var application = _dataContext.Applications.SingleOrDefault(app => app.Id == request.ApplicationId);

            if (application != null)
            {
                var applicationData = JObject.Parse(application.ApplicationData ?? "{}");

                var section = _dataContext.ApplicationSections.AsNoTracking().SingleOrDefault(sec => sec.Id == request.SectionId && sec.ApplicationId == application.Id);
                var page = section?.QnAData?.Pages.SingleOrDefault(p => p.PageId == request.PageId);

                if (page != null)
                {
                    var questionTagsWhichHaveBeenUpdated = new List<string>();

                    foreach (var question in page.Questions)
                    {
                        SetApplicationDataField(question, applicationData);
                        if (!string.IsNullOrWhiteSpace(question.QuestionTag)) questionTagsWhichHaveBeenUpdated.Add(question.QuestionTag);

                        if (question.Input.Options != null)
                        {
                            foreach (var option in question.Input.Options.Where(o => o.FurtherQuestions != null))
                            {
                                foreach (var furtherQuestion in option.FurtherQuestions)
                                {
                                    SetApplicationDataField(furtherQuestion, applicationData);
                                    if (!string.IsNullOrWhiteSpace(furtherQuestion.QuestionTag)) questionTagsWhichHaveBeenUpdated.Add(furtherQuestion.QuestionTag);
                                }
                            }
                        }
                    }

                    application.ApplicationData = applicationData.ToString(Formatting.None);
                    _dataContext.SaveChanges();

                    SetStatusOfAllPagesBasedOnUpdatedQuestionTags(application.Id, questionTagsWhichHaveBeenUpdated);
                }
            }
        }

        private void SetStatusOfAllPagesBasedOnUpdatedQuestionTags(Guid applicationId, List<string> questionTags)
        {
            if (questionTags != null && questionTags.Count > 0)
            {
                var sections = _dataContext.ApplicationSections.Where(sec => sec.ApplicationId == applicationId);

                // Go through each section in the application
                foreach (var section in sections)
                {
                    // Have to force QnAData a new object and reassign for Entity Framework to pick up changes
                    var qnaData = new QnAData(section.QnAData);

                    // Get the list of pages that contain one of QuestionTags in the next condition
                    var pagesToProcess = new List<Page>();
                    foreach (var questionTag in questionTags.Distinct())
                    {
                        var questionTagPages = qnaData.Pages.Where(p => !p.AllowMultipleAnswers && p.Next.SelectMany(n => n.Conditions).Select(c => c.QuestionTag).Contains(questionTag));
                        pagesToProcess.AddRange(questionTagPages);
                    }

                    // Deactivate & Activate affected pages accordingly
                    foreach (var page in pagesToProcess)
                    {
                        if (page.PageOfAnswers != null && page.PageOfAnswers.Count > 0)
                        {
                            var nextAction = GetNextActionForPage(section.Id, page.PageId);
                            if (nextAction?.Conditions != null)
                            {
                                DeactivateDependentPages(nextAction, page.PageId, qnaData, page);
                                ActivateDependentPages(nextAction, page.PageId, qnaData);
                            }
                        }
                    }

                    // Assign QnAData back so Entity Framework will pick up changes
                    section.QnAData = qnaData;
                }

                _dataContext.SaveChanges();
            }
        }

        private static void SetApplicationDataField(Question question, JObject applicationData)
        {
            if (question != null && applicationData != null)
            {
                var questionTag = question.QuestionTag;
                string questionTagAnswer = null;

                if (!string.IsNullOrWhiteSpace(questionTag))
                {
                    if (applicationData.ContainsKey(question.QuestionTag))
                    {
                        applicationData[question.QuestionTag] = questionTagAnswer;
                    }
                    else
                    {
                        applicationData.Add(question.QuestionTag, new JValue(questionTagAnswer));
                    }
                }
            }
        }
    }
}
