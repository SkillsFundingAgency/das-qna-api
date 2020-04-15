using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.SetPageAnswers
{
    public class SetPageAnswersHandler : SetAnswersBase, IRequestHandler<SetPageAnswersRequest, HandlerResponse<SetPageAnswersResponse>>
    {
        private readonly IAnswerValidator _answerValidator;
        private readonly ITagProcessingService _tagProcessingService;

        public SetPageAnswersHandler(QnaDataContext dataContext, IAnswerValidator answerValidator, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService) : base(dataContext, notRequiredProcessor, tagProcessingService)
        {
            _answerValidator = answerValidator;
            _tagProcessingService = tagProcessingService;
        }

        public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(SetPageAnswersRequest request, CancellationToken cancellationToken)
        {
            var validationErrorResponse = ValidateRequest(request);

            if(validationErrorResponse != null)
            {
                return validationErrorResponse;
            }
 
            SaveAnswersIntoPage(request);
            UpdateApplicationData(request);

            var nextAction = GetNextActionForPage(request.SectionId, request.PageId);
            var checkboxListAllNexts = GetCheckboxListMatchingNextActionsForPage(request.SectionId, request.PageId);
            
            SetStatusOfNextPagesBasedOnDeemedNextActions(request.SectionId, request.PageId, nextAction, checkboxListAllNexts);
            
            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));
        }

        private HandlerResponse<SetPageAnswersResponse> ValidateRequest(SetPageAnswersRequest request)
        {
            var section =  _dataContext.ApplicationSections.AsNoTracking().SingleOrDefault(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId);
            var page = section?.QnAData?.Pages.SingleOrDefault(p => p.PageId == request.PageId);

            var answers = GetAnswersFromRequest(request);

            if (page is null)
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "Cannot find requested page.");
            }
            else if(answers is null || !answers.Any())
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "No answers specified.");
            }
            else if(answers.Any(a => a.QuestionId is null))
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "All answers must specify which question they are related to.");
            }
            else if (page.AllowMultipleAnswers)
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for Multiple Answers pages. Use AddAnswer / RemoveAnswer instead.");
            }
            else if (page.Questions.Any())
            {
                if (page.Questions.All(q => "FileUpload".Equals(q.Input?.Type, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for FileUpload questions. Use Upload / DeleteFile instead.");
                }
                else if (page.Questions.Any(q => "FileUpload".Equals(q.Input?.Type, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "Pages cannot contain a mixture of FileUploads and other Question Types.");
                }
                else if (page.Questions.Count > answers.Count)
                {
                    return new HandlerResponse<SetPageAnswersResponse>(success: false, message: $"Number of Answers supplied ({answers.Count}) does not match number of first level Questions on page ({page.Questions.Count}).");
                }

                var validationErrors = _answerValidator.Validate(answers, page);
                if (validationErrors.Any())
                {
                    return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(validationErrors));
                }
            }

            return null;
        }

        private void SaveAnswersIntoPage(SetPageAnswersRequest request)
        {
            var section = _dataContext.ApplicationSections.SingleOrDefault(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId);

            if (section != null)
            {
                // Have to force QnAData a new object and reassign for Entity Framework to pick up changes
                var qnaData = new QnAData(section.QnAData);
                var page = qnaData?.Pages.SingleOrDefault(p => p.PageId == request.PageId);

                if (page != null)
                {
                    var answers = GetAnswersFromRequest(request);
                    page.PageOfAnswers = new List<PageOfAnswers>(new[] { new PageOfAnswers() { Answers = answers } });

                    MarkPageAsComplete(page);
                    MarkPageFeedbackAsComplete(page);

                    // Assign QnAData back so Entity Framework will pick up changes
                    section.QnAData = qnaData;
                    _dataContext.SaveChanges();
                }
            }
        }

        private void UpdateApplicationData(SetPageAnswersRequest request)
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
                    var answers = GetAnswersFromRequest(request);

                    foreach (var question in page.Questions)
                    {
                        SetApplicationDataField(question, answers, applicationData);
                        if (!string.IsNullOrWhiteSpace(question.QuestionTag))
                            questionTagsWhichHaveBeenUpdated.Add(question.QuestionTag);

                        if (question.Input.Options != null)
                        {
                            foreach (var option in question.Input.Options.Where(o => o.FurtherQuestions != null))
                            {
                                foreach (var furtherQuestion in option.FurtherQuestions)
                                {
                                    SetApplicationDataField(furtherQuestion, answers, applicationData);
                                    if (!string.IsNullOrWhiteSpace(furtherQuestion.QuestionTag))
                                        questionTagsWhichHaveBeenUpdated.Add(furtherQuestion.QuestionTag);
                                }
                            }
                        }
                    }

                    application.ApplicationData = applicationData.ToString(Formatting.None);
                    _dataContext.SaveChanges();

                    SetStatusOfAllPagesBasedOnUpdatedQuestionTags(application.Id, questionTagsWhichHaveBeenUpdated);
                    _tagProcessingService.ClearDeactivatedTags(application.Id, request.SectionId);
      
                }
            }
        }

        private static void SetApplicationDataField(Question question, List<Answer> answers, JObject applicationData)
        {
            if (question != null && applicationData != null)
            {
                var questionTag = question.QuestionTag;
                var questionTagAnswer = answers?.SingleOrDefault(a => a.QuestionId == question.QuestionId)?.Value;

                if (!string.IsNullOrWhiteSpace(questionTag))
                {
                    if (applicationData.ContainsKey(questionTag))
                    {
                        applicationData[questionTag] = questionTagAnswer;
                    }
                    else
                    {
                        applicationData.Add(questionTag, new JValue(questionTagAnswer));
                    }
                }
            }
        }

        private static List<Answer> GetAnswersFromRequest(SetPageAnswersRequest request)
        {
            var answers = new List<Answer>();

            if (request.Answers != null)
            {
                answers.AddRange(request.Answers);
            }

            return answers;
        }
    }
}