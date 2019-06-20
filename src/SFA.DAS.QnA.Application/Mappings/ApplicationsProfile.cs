using AutoMapper;
using SFA.DAS.QnA.Application.Queries.GetWorkflows;
using SFA.DAS.QnA.Application.Queries.Sequences;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequences;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Mappings
{
    public class ApplicationsProfile : Profile
    {
        public ApplicationsProfile()
        {
            CreateMap<Workflow, WorkflowResponse>();
            CreateMap<ApplicationSequence, SequenceResponse>()
                .ForMember(response => response.SequenceNumber, opt => opt.MapFrom(applicationSequence => applicationSequence.SequenceId));
        }
    }
}