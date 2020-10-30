using System;

namespace SFA.DAS.QnA.Data.Entities
{
    public class ApplicationAnswer
    {
        public long Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid SectionId { get; set; }
        public string PageId { get; set; }
        public string QuestionId { get; set; }
        public string Value { get; set; }
    }

}