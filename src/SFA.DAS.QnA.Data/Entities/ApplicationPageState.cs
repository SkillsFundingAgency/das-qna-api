using System;

namespace SFA.DAS.QnA.Data.Entities
{
    public class ApplicationPageState
    {
        public long Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid SectionId { get; set; }
        public string PageId { get; set; }
        public bool Complete { get; set; }
        public bool Active { get; set; }
        public bool NotRequired { get; set; }
    }
}