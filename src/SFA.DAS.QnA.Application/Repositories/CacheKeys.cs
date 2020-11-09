using System;

namespace SFA.DAS.QnA.Application.Repositories
{
    public class CacheKeys
    {
        private const string WorkflowByIdPrefix = "_workflow_by_id_";
        private const string WorkflowByTypePrefix = "_workflow_by_type_";
        private const string WorkflowSectionPrefix = "_workflow_section_";
        private const string ApplicationPrefix = "_application_";
        private const string WorkflowSectionIdsPrefix = "_workflow_section_ids_";
        private const string WorkflowSequencesPrefix = "_workflow_sequences_";

        public static string WorkflowByIdKey(Guid workflowId) => $"{WorkflowByIdPrefix}{workflowId:D}";
        public static string WorkflowByTypeKey(string workflowType) => $"{WorkflowByTypePrefix}{workflowType.ToLower()}";
        public static string WorkflowSectionKey(Guid workflowSectionId) => $"{WorkflowSectionPrefix}{workflowSectionId:D}";
        public static string ApplicationKey(Guid applicationId) => $"{ApplicationPrefix}{applicationId:D}";
        public static string WorkflowSectionIds(Guid workflowId) => $"{WorkflowSectionIdsPrefix}{workflowId:D}";
        public static string WorkflowSequencesKey(Guid workflowId) => $"{WorkflowSequencesPrefix}{workflowId:D}";
    }
}