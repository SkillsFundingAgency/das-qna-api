using System;
using SFA.DAS.Qna.Data;

namespace SFA.DAS.Qna.Api.Types
{
    public class Section
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public Guid ApplicationId { get; set; }
        public QnAData QnAData { get; set; }
        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public string DisplayType { get; set; }
        public bool NotRequired { get; set; }
    }
}