using System.Collections.Generic;

namespace SFA.DAS.Qna.Api.Types.Page
{
    public class QnAData
    {
        public bool? RequestedFeedbackAnswered { get; set; }
        public List<Page> Pages { get; set; }
        public FinancialApplicationGrade FinancialApplicationGrade { get; set; }
    }
}