using System;

namespace SFA.DAS.QnA.Data.Entities
{
    public class WorkflowSequence : EntityBase
    {
        public Guid WorkflowId { get; set; }
        public int SequenceId { get; set; }
        public int SectionId { get; set; }
        public bool IsActive { get; set; }
    }
}