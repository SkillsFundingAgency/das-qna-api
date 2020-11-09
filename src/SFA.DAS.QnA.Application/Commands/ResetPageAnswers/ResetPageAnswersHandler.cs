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
using SFA.DAS.QnA.Application.Repositories;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.ResetPageAnswers
{
    public class ResetPageAnswersHandler : SetAnswersBase, IRequestHandler<ResetPageAnswersRequest, HandlerResponse<ResetPageAnswersResponse>>
    {
        public ResetPageAnswersHandler(QnaDataContext dataContext, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService,  IApplicationAnswersRepository applicationAnswersRepository, IWorkflowRepository workflowRepository) 
            : base(dataContext, notRequiredProcessor, tagProcessingService, null, applicationAnswersRepository, workflowRepository)
        {
        }

        public async Task<HandlerResponse<ResetPageAnswersResponse>> Handle(ResetPageAnswersRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.AsNoTracking().FirstOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken: cancellationToken);
            if (application is null) return new HandlerResponse<ResetPageAnswersResponse>(false, "Application does not exist");


            var sequenceSection = await _dataContext.WorkflowSections
                .Join(_dataContext.WorkflowSequences,
                    wsec => wsec.Id,
                    wseq => wseq.SectionId,
                    (sec, sequence) => new {Section = sec, Sequence = sequence})
                .Where(x => x.Section.Id == request.SectionId && x.Sequence.WorkflowId == application.WorkflowId)
                .FirstOrDefaultAsync(cancellationToken);


            var page = sequenceSection.Section.QnAData.Pages.FirstOrDefault(x => x.PageId == request.PageId);

            var section = new ApplicationSection
            {
                ApplicationId = request.ApplicationId,
                DisplayType = sequenceSection.Section.DisplayType,
                Id = sequenceSection.Section.Id,
                LinkTitle = sequenceSection.Section.LinkTitle,
                QnAData = sequenceSection.Section.QnAData,
                SectionNo = sequenceSection.Sequence.SectionNo,
                SequenceNo = sequenceSection.Sequence.SequenceNo,
                Title = sequenceSection.Section.Title
            };
            var validationErrorResponse = ValidateRequest(request, section);

            if (validationErrorResponse != null)
            {
                return validationErrorResponse;
            }

            ResetPageAnswers(request, section);

            UpdateApplicationData(request, application, section);

            var nextAction = GetNextActionForPage(section, application, request.PageId);
            var checkboxListAllNexts = GetCheckboxListMatchingNextActionsForPage(section, application, request.PageId);

            SetStatusOfNextPagesBasedOnDeemedNextActions(section, request.PageId, nextAction, checkboxListAllNexts);

            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<ResetPageAnswersResponse>(new ResetPageAnswersResponse(true));



            //var section = await _dataContext.ApplicationSections.SingleOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            //var validationErrorResponse = ValidateRequest(request, section);

            //if (validationErrorResponse != null)
            //{
            //    return validationErrorResponse;
            //}

            //ResetPageAnswers(request, section);

            //var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
            //UpdateApplicationData(request, application, section);

            //var nextAction = GetNextActionForPage(section, application, request.PageId);
            //var checkboxListAllNexts = GetCheckboxListMatchingNextActionsForPage(section, application, request.PageId);

            //SetStatusOfNextPagesBasedOnDeemedNextActions(section, request.PageId, nextAction, checkboxListAllNexts);

            //await _dataContext.SaveChangesAsync(cancellationToken);

            //return new HandlerResponse<ResetPageAnswersResponse>(new ResetPageAnswersResponse(true));
        }

        private HandlerResponse<ResetPageAnswersResponse> ValidateRequest(ResetPageAnswersRequest request, ApplicationSection section)
        {
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

        private void ResetPageAnswers(ResetPageAnswersRequest request, ApplicationSection section)
        {
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
                }
            }
        }

        private void UpdateApplicationData(ResetPageAnswersRequest request, Data.Entities.Application application, ApplicationSection section)
        {
            if (application != null)
            {
                var applicationData = JObject.Parse(application.ApplicationData ?? "{}");

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
                    
                    SetStatusOfAllPagesBasedOnUpdatedQuestionTags(application, questionTagsWhichHaveBeenUpdated);
                    _tagProcessingService.ClearDeactivatedTags(application.Id, request.SectionId);

                }
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
