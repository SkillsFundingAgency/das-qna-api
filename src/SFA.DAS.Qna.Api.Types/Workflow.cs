using System;

namespace SFA.DAS.Qna.Api.Types
{
    public class Workflow
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }
    }
}