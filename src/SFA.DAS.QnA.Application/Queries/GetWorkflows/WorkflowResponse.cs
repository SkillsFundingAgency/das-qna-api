using System;

namespace SFA.DAS.QnA.Application.Queries.GetWorkflows
{
    public class WorkflowResponse
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }
    }
}