using System;

namespace SFA.DAS.QnA.Data.Entities
{
    public class Workflow : EntityBase
    {
        public Guid ProjectId { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }
        public string ReferenceFormat { get; set; }
    }
}