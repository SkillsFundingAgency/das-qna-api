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
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.SetPageAnswers
{
    public class SetPageAnswersHandler : SetAnswersBase, IRequestHandler<SetPageAnswersRequest, HandlerResponse<SetPageAnswersResponse>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IAnswerValidator _answerValidator;

        public SetPageAnswersHandler(QnaDataContext dataContext, IAnswerValidator answerValidator)
        {
            _dataContext = dataContext;
            _answerValidator = answerValidator;
        }

        public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(SetPageAnswersRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            var qnaData = new QnAData(section.QnAData);
            var page = qnaData.Pages.FirstOrDefault(p => p.PageId == request.PageId);

            if (page.AllowMultipleAnswers) return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for Multiple Answers pages. Use AddAnswer / RemoveAnswer instead.");

            if (page.Questions.Count > 0)
            {
                if (page.Questions.Count > request.Answers.Count)
                {
                    return new HandlerResponse<SetPageAnswersResponse>(success: false, message: $"Number of Answers supplied ({request.Answers.Count}) does not match number of first level Questions on page ({page.Questions.Count}).");
                }

                // Re-inject current FileUpload answer if one wasn't specified (Note that calling application should be using the Upload endpoint and not SetPageAnswers!)
                foreach (var question in page.Questions.Where(q => q.Input.Type == "FileUpload"))
                {
                    var answerToThisQuestion = request.Answers.SingleOrDefault(a => a.QuestionId == question.QuestionId);

                    if (answerToThisQuestion != null && string.IsNullOrEmpty(answerToThisQuestion?.Value))
                    {
                        var currentAnswerToThisQuestion = page.PageOfAnswers?.SelectMany(p => p.Answers).SingleOrDefault(a => a.QuestionId == question.QuestionId);
                        if (!string.IsNullOrEmpty(currentAnswerToThisQuestion?.Value))
                        {
                            answerToThisQuestion.QuestionId = currentAnswerToThisQuestion.QuestionId;
                            answerToThisQuestion.Value = currentAnswerToThisQuestion.Value;
                        }
                    }
                }

                var validationErrors = _answerValidator.Validate(request.Answers, page);
                if (validationErrors.Any())
                {
                    return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(validationErrors));
                }
            }

            page.Complete = true;

            MarkFeedbackComplete(page);

            var nextAction = GetNextAction(page, request.Answers, section, _dataContext);

            var checkboxListAllNexts = GetCheckboxListMatchingNextActions(page, request.Answers, section, _dataContext);
            
            SetStatusOfNextPagesBasedOnAnswer(qnaData, page, request.Answers, nextAction, checkboxListAllNexts);

            await SaveAnswersIntoPage(request, cancellationToken, qnaData, section);

            await UpdateApplicationData(request.ApplicationId, page, request.Answers);
            
            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));
        }

        private async Task UpdateApplicationData(Guid applicationId, Page page, List<Answer> answers)
        {
            var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == applicationId);
            var applicationData = JObject.Parse(application.ApplicationData);
            foreach (var question in page.Questions)
            {
                SetApplicationDataField(answers, applicationData, question);

                if (question.Input.Options == null) continue;

                foreach (var option in question.Input.Options.Where(o => o.FurtherQuestions != null))
                {
                    foreach (var furtherQuestion in option.FurtherQuestions)
                    {
                        SetApplicationDataField(answers, applicationData, furtherQuestion);
                    }
                }
            }

            application.ApplicationData = applicationData.ToString(Formatting.None);

            await _dataContext.SaveChangesAsync();
        }

        private static void SetApplicationDataField(List<Answer> answers, JObject applicationData, Question question)
        {
            if (string.IsNullOrWhiteSpace(question.QuestionTag)) return;

            if (applicationData.ContainsKey(question.QuestionTag))
            {
                applicationData[question.QuestionTag] = answers.Single(a => a.QuestionId == question.QuestionId).Value;
            }
            else
            {
                applicationData.Add(question.QuestionTag, new JValue(answers.Single(a => a.QuestionId == question.QuestionId).Value));
            }
        }


        private async Task SaveAnswersIntoPage(SetPageAnswersRequest request, CancellationToken cancellationToken, QnAData qnaData, ApplicationSection section)
        {
            qnaData.Pages.Single(p => p.PageId == request.PageId).PageOfAnswers = new List<PageOfAnswers>(new[] {new PageOfAnswers() {Answers = request.Answers}});
            section.QnAData = qnaData;

            await _dataContext.SaveChangesAsync(cancellationToken);
        }

        
    }
}