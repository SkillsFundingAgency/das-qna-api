using AutoMapper;
using SFA.DAS.Qna.Api.Types;
using SFA.DAS.QnA.Application.Queries.GetWorkflows;
using SFA.DAS.QnA.Application.Queries.Sequences;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequences;
using SFA.DAS.QnA.Data.Entities;
using Workflow = SFA.DAS.Qna.Api.Types.Workflow;

namespace SFA.DAS.QnA.Application.Mappings
{
    public class ApplicationsProfile : Profile
    {
        public ApplicationsProfile()
        {
            CreateMap<Data.Entities.Workflow, Workflow>();
            CreateMap<ApplicationSequence, Sequence>();
            CreateMap<ApplicationSection, Section>();
        }
    }
}