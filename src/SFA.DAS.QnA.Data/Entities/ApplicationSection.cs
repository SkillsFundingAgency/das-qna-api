using System;
using SFA.DAS.Qna.Data;

namespace SFA.DAS.QnA.Data.Entities
{
    public class ApplicationSection 
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public Guid ApplicationId { get; set; }
        public int SectionId { get; set; }
        public int SequenceId { get; set; }

        public QnAData QnAData { get; set; }

        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public string DisplayType { get; set; }
        public bool NotRequired { get; set; }
    }
}