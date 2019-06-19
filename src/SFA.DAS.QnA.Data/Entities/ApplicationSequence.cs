using System;
using System.Collections.Generic;

namespace SFA.DAS.QnA.Data.Entities
{
    public class ApplicationSequence : EntityBase
    {
        public Guid ApplicationId { get; set; }
        public int SequenceId { get; set; }
        public bool IsActive { get; set; }
        public List<ApplicationSection> Sections { get; set; }
        public bool NotRequired { get; set; }
    }
}