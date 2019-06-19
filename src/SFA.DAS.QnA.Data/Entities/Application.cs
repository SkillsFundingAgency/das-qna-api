using System;

namespace SFA.DAS.QnA.Data.Entities
{
    public class Application : EntityBase
    {
        public DateTime WithdrawnAt { get; set; }
        public string WithdrawnBy { get; set; }
        public string ApplicationStatus { get; set; }
        public Guid CreatedFromWorkflowId { get; set; }
    }
}