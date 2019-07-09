using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SFA.DAS.Qna.Api.Types;
using SFA.DAS.Qna.Api.Types.Page;
using SFA.DAS.Qna.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.SetPageAnswers
{
    public class SetPageAnswersHandler : IRequestHandler<SetPageAnswersRequest, HandlerResponse<SetPageAnswersResponse>>
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

            var validationErrors = _answerValidator.Validate(request.Answers, page);
            if (validationErrors.Any())
            {
                return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(validationErrors));
            }

            page.Complete = true;

            MarkFeedbackComplete(page);

            var nextAction = GetNextAction(page, request.Answers, section);

            SetStatusOfNextPagesBasedOnAnswer(qnaData, page, request.Answers, nextAction);

            await SaveAnswersIntoPage(request, cancellationToken, qnaData, section);

            await UpdateApplicationData(request.ApplicationId, page, request.Answers);
            
            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));
        }

        private async Task UpdateApplicationData(Guid applicationId, Page page, List<Answer> answers)
        {
            var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == applicationId);
            var applicationData = JObject.Parse(application.ApplicationData);
            foreach (var question in page.Questions.Where(q => !string.IsNullOrWhiteSpace(q.QuestionTag)))
            {
                if (applicationData.ContainsKey(question.QuestionTag))
                {
                    applicationData[question.QuestionTag] = answers.Single(a => a.QuestionId == question.QuestionId).Value;
                }
                else
                {
                    applicationData.Add(question.QuestionTag, new JValue(answers.Single(a => a.QuestionId == question.QuestionId).Value));
                }
            }

            await _dataContext.SaveChangesAsync();
        }

        private Next GetNextAction(Page page, List<Answer> answers, ApplicationSection section)
        {
            if (page.Next is null || !page.Next.Any())
            {
                throw new ApplicationException($"Page {page.PageId}, in Sequence {page.SequenceId}, Section {page.SectionId} has no 'Next' instructions.");
            }

            if (page.Next.Count == 1)
            {
                return page.Next.First();
            }

            foreach (var next in page.Next)
            {
                if (next.Condition != null)
                {
                    var answer = answers.Single(a => a.QuestionId == next.Condition.QuestionId);
                    if (answer.QuestionId == next.Condition.QuestionId && answer.Value == next.Condition.MustEqual)
                    {
                        return next;
                    }
                }
                else
                {
                    return next;
                }
            }

            throw new ApplicationException($"Page {page.PageId}, in Sequence {page.SequenceId}, Section {page.SectionId} is missing a matching 'Next' instruction for Application {section.ApplicationId}");
        }

        private void SetStatusOfNextPagesBasedOnAnswer(QnAData qnaData, Page page, List<Answer> answers, Next nextAction)
        {
            var hasConditionalBranch = page.Next.Any(n => n.Condition != null);
            if (!hasConditionalBranch) return;

            if (page.PageOfAnswers != null && page.PageOfAnswers.Count > 0)
            {
                var existingAnswer = page.PageOfAnswers?[0].Answers.SingleOrDefault(a => a.QuestionId == nextAction.Condition.QuestionId);

                if (existingAnswer != answers.Single(a => a.QuestionId == nextAction.Condition.QuestionId))
                {
                    DeactivateDependentPages(page.PageId, qnaData, page, nextAction);
                }
            }
            
            ActivateDependentPages(nextAction, page.PageId, qnaData);
            
//                else
//                {
//                    var nextPage = qnaData.Pages.Single(p => p.PageId == next.ReturnId);
//                    nextPage.Active = false;
//                }
            
        }

        private void DeactivateDependentPages(string branchingPageId, QnAData qnaData, Page page, Next chosenAction)
        {
            foreach (var nextAction in page.Next.Where(n => n != chosenAction))
            {
                if (nextAction.Action == "NextPage")
                {
                    var nextPage = qnaData.Pages.Single(p => p.PageId == nextAction.ReturnId);
                    if (nextPage.ActivatedByPageId == branchingPageId)
                    {
                        nextPage.Active = false;
                    }
                    
                    foreach (var thisPagesNext in nextPage.Next)
                    {
                        DeactivateDependentPages(branchingPageId, qnaData, nextPage, chosenAction);
                    }
                }
            }
        }

        private void ActivateDependentPages(Next next, string branchingPageId, QnAData qnaData)
        {
            if (next.Action == "NextPage")
            {
                var nextPage = qnaData.Pages.Single(p => p.PageId == next.ReturnId);
                if (nextPage.ActivatedByPageId == branchingPageId)
                {
                    nextPage.Active = true;
                }

                foreach (var thisPagesNext in nextPage.Next)
                {
                    ActivateDependentPages(thisPagesNext, branchingPageId, qnaData);
                }
            }
        }

        private async Task SaveAnswersIntoPage(SetPageAnswersRequest request, CancellationToken cancellationToken, QnAData qnaData, ApplicationSection section)
        {
            qnaData.Pages.Single(p => p.PageId == request.PageId).PageOfAnswers = new List<PageOfAnswers>(new[] {new PageOfAnswers() {Answers = request.Answers}});
            section.QnAData = qnaData;

            await _dataContext.SaveChangesAsync(cancellationToken);
        }

        private static void MarkFeedbackComplete(Page page)
        {
            if (page.HasFeedback)
            {
                page.Feedback.ForEach(f => f.IsCompleted = true);
            }
        }
    }
}