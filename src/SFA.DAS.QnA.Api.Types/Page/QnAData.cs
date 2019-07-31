using System.Collections.Generic;

namespace SFA.DAS.QnA.Api.Types.Page
{
    public class QnAData
    {
        public QnAData(){}

        public QnAData(QnAData copyFrom)
        {
            this.RequestedFeedbackAnswered = copyFrom.RequestedFeedbackAnswered;
            this.Pages = copyFrom.Pages;
        }
        
        public bool? RequestedFeedbackAnswered { get; set; }
        public List<Page> Pages { get; set; }
    }
}