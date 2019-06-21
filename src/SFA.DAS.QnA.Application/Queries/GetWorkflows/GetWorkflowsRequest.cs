using System.Collections.Generic;
using MediatR;
using SFA.DAS.Qna.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.GetWorkflows
{
    public class GetWorkflowsRequest : IRequest<List<Workflow>>
    {
        
    }
}