using System;

namespace SFA.DAS.QnA.Data.Entities
{
    public class WorkflowSequence
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public Guid WorkflowId { get; set; }
        public int SequenceId { get; set; }
        public bool IsActive { get; set; }
    }
}