using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Application.Queries.GetWorkflows;
using SFA.DAS.Qna.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("/workflows")]
    [Produces("application/json")]
    public class WorkflowsController : Controller
    {
        private readonly IMediator _mediator;

        public WorkflowsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Returns all of the current live workflows
        /// </summary>
        /// <returns>An array of workflows</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<WorkflowResponse>>> GetWorkflows()
        {
            var workflows = await _mediator.Send(new GetWorkflowsRequest());
            return workflows;
        }
    }
}