using System;
using SFA.DAS.Qna.Api.Types.Page;

namespace SFA.DAS.QnA.Data.Entities
{
    public class WorkflowSection
    {
        public Guid? Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public string Status { get; set; }
        public string DisplayType { get; set; }
        public QnAData QnAData { get; set; }
    }
}