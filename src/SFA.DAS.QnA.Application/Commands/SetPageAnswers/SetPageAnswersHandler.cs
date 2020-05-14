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

        public SetPageAnswersHandler(QnaDataContext dataContext, IAnswerValidator answerValidator, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService) : base(dataContext, notRequiredProcessor)
        {
            _answerValidator = answerValidator;
            _tagProcessingService = tagProcessingService;
        }

        public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(SetPageAnswersRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.SingleOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            var validationErrorResponse = ValidateRequest(request, section);

            if(validationErrorResponse != null)
            {
                return validationErrorResponse;
            }

            SaveAnswersIntoPage(section, request);

            var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
            UpdateApplicationData(request, section, application);

            var nextAction = GetNextActionForPage(section, application, request.PageId);
            var checkboxListAllNexts = GetCheckboxListMatchingNextActionsForPage(section, application, request.PageId);
            
            SetStatusOfNextPagesBasedOnDeemedNextActions(section, request.PageId, nextAction, checkboxListAllNexts);

            await _dataContext.SaveChangesAsync(cancellationToken);
            
            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));
        }

        private HandlerResponse<SetPageAnswersResponse> ValidateRequest(SetPageAnswersRequest request, ApplicationSection section)
        {
            var page = section?.QnAData?.Pages.SingleOrDefault(p => p.PageId == request.PageId);

            if (page is null)
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "Cannot find requested page.");
            }
            else if(request.Answers is null)
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "No answers specified.");
            }
            else if(request.Answers.Any(a => a.QuestionId is null))
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "All answers must specify which question they are related to.");
            }
            else if (page.AllowMultipleAnswers)
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for Multiple Answers pages. Use AddAnswer / RemoveAnswer instead.");
            }
            else if (page.Questions.Any())
            {
                var answers = GetAnswersFromRequest(request);

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

        private void SaveAnswersIntoPage(ApplicationSection section, SetPageAnswersRequest request)
        {
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
                }
            }
        }

        private void UpdateApplicationData(SetPageAnswersRequest request, ApplicationSection section, Data.Entities.Application application)
        {
            if (application != null)
            {
                var applicationData = JObject.Parse(application.ApplicationData ?? "{}");

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

                    SetStatusOfAllPagesBasedOnUpdatedQuestionTags(application, questionTagsWhichHaveBeenUpdated);
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