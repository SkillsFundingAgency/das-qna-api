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
                
                var validationErrors = _answerValidator.Validate(request.Answers, page);
                if (validationErrors.Any())
                {
                    return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(validationErrors));
                }
            }

            page.Complete = true;

            MarkFeedbackComplete(page);

            var nextAction = GetNextAction(page, request.Answers, section, _dataContext);

            SetStatusOfNextPagesBasedOnAnswer(qnaData, page, request.Answers, nextAction);

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

            var answer = answers.Single(a => a.QuestionId == question.QuestionId);
            
            if (applicationData.ContainsKey(question.QuestionTag))
            {
                applicationData[question.QuestionTag] = answer.Value.Length == 1 ? (JToken) answer.Value[0] : new JValue(answer.Value[0]);
            }
            else
            {
                applicationData.Add(question.QuestionTag, answer.Value.Length == 1 ? new JValue(answer.Value[0]) : JToken.FromObject(answer.Value));
            }
        }

        

        private void SetStatusOfNextPagesBasedOnAnswer(QnAData qnaData, Page page, List<Answer> answers, Next nextAction)
        {
            var hasConditionalBranch = page.Next.Any(n => n.Conditions != null && n.Conditions.Any());
            if (!hasConditionalBranch || nextAction == null || (nextAction.Conditions == null && nextAction.Conditions.Any())) return;

            if (page.PageOfAnswers != null && page.PageOfAnswers.Count > 0)
            {
                var existingAnswer = page.PageOfAnswers?[0].Answers.SingleOrDefault(a => a.QuestionId == answers[0].QuestionId);

                if (existingAnswer != null && existingAnswer != answers.Single(a => a.QuestionId == answers[0].QuestionId))
                {
                    DeactivateDependentPages(page.PageId, qnaData, page, nextAction);
                }
            }
            
            ActivateDependentPages(nextAction, page.PageId, qnaData);
        }

        private void DeactivateDependentPages(string branchingPageId, QnAData qnaData, Page page, Next chosenAction)
        {
            foreach (var nextAction in page.Next.Where(n => n != chosenAction))
            {
                if (nextAction.Action != "NextPage") continue;
                
                var nextPage = qnaData.Pages.FirstOrDefault(p => p.PageId == nextAction.ReturnId);
                if (nextPage == null)
                {
                    break;
                }
                if (nextPage.ActivatedByPageId == branchingPageId)
                {
                    nextPage.Active = false;
                }
                    
                foreach (var thisPagesNext in nextPage.Next)
                {
                    DeactivateDependentPages(branchingPageId, qnaData, nextPage, thisPagesNext);
                }
            }
        }

        private void ActivateDependentPages(Next next, string branchingPageId, QnAData qnaData)
        {
            if (next.Action != "NextPage") return;
            
            var nextPage = qnaData.Pages.FirstOrDefault(p => p.PageId == next.ReturnId);
            if (nextPage == null)
            {
                return;
            }
            if (nextPage.ActivatedByPageId == branchingPageId)
            {
                nextPage.Active = true;
            }

            foreach (var thisPagesNext in nextPage.Next)
            {
                ActivateDependentPages(thisPagesNext, branchingPageId, qnaData);
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