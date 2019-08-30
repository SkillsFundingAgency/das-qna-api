using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.SetPageAnswers
{
    public class SetAnswersBase
    {
        protected Next GetNextAction(Page page, List<Answer> answers, ApplicationSection section)
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
        
        protected void MarkFeedbackComplete(Page page)
        {
            if (page.HasFeedback)
            {
                page.Feedback.ForEach(f => f.IsCompleted = true);
            }
        }
    }
}