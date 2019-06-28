using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

            var validationErrors = _answerValidator.Validate(request, page);
            if (validationErrors.Any())
            {
                return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(validationErrors));
            }

            page.Complete = true;

            MarkFeedbackComplete(page);

            await SaveAnswersIntoPage(request, cancellationToken, qnaData, section);

            SetStatusOfNextPagesBasedOnAnswer(section, page, request.Answers);
            
            // If validation passes.....
            // Get next action...
            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse("REPLACETHIS", "REPLACETHIS"));

            // return response
        }

        private void SetStatusOfNextPagesBasedOnAnswer(ApplicationSection section, Page page, List<Answer> answers)
        {
            var hasConditionalBranch = page.Next.Any(n => n.Condition != null);
            if (!hasConditionalBranch) return;

            var conditionMet = false;
//            foreach (var next in page.Next)
//            {
//                if (next.Condition != null)
//                {
//                    var answer = answers.Single(a => a.QuestionId == nextCondition.Condition.QuestionId);
//                    if (answer.Value == nextCondition.Condition.MustEqual)
//                    {
//                        // Set next page Active = true
//                    }
//                }
//                
//                else
//                {
//                    
//                }
//            }
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