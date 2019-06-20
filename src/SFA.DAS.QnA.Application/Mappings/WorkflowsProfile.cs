using AutoMapper;
using SFA.DAS.QnA.Application.Queries.GetWorkflows;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Mappings
{
    public class WorkflowsProfile : Profile
    {
        public WorkflowsProfile()
        {
            CreateMap<Workflow, WorkflowResponse>();
        }
    }
}