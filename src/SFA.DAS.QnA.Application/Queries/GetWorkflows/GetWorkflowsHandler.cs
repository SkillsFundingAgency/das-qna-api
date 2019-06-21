using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Qna.Data;
using SFA.DAS.QnA.Data.Entities;
using Workflow = SFA.DAS.Qna.Api.Types.Workflow;

namespace SFA.DAS.QnA.Application.Queries.GetWorkflows
{
    public class GetWorkflowsHandler : IRequestHandler<GetWorkflowsRequest, List<Workflow>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;

        public GetWorkflowsHandler(QnaDataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        
        public async Task<List<Workflow>> Handle(GetWorkflowsRequest request, CancellationToken cancellationToken)
        {
            var workflows = await _dataContext.Workflows.Where(w => w.Status == WorkflowStatus.Live).ToListAsync(cancellationToken: cancellationToken);

            var responses =  _mapper.Map<List<Workflow>>(workflows);
            
            return responses;
        }
    }
}