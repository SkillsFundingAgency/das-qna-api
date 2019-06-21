using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.QnA.Application.Queries.GetWorkflows
{
    public class GetWorkflowsRequest : IRequest<List<WorkflowResponse>>
    {
        
    }
}