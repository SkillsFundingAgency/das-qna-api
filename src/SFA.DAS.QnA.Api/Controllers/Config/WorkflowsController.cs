using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Api.Controllers.Config
{
    [Route("/config/projects")]
    public class WorkflowsController : Controller
    {
        private readonly IMediator _mediator;

        public WorkflowsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{projectId}/workflows")]
        public async Task<ActionResult<List<WorkflowSection>>> GetWorkflowSections(Guid projectId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{projectId}/workflows/{sectionId}")]
        public async Task<ActionResult<WorkflowSection>> GetWorkflowSection(Guid projectId, Guid sectionId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{projectId}/workflows/{sectionId}")]
        public async Task<ActionResult<WorkflowSection>> UpsertWorkflowSection(Guid projectId, Guid sectionId, [FromBody] WorkflowSection section)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{projectId}/workflows")]
        public async Task<ActionResult<WorkflowSection>> CreateWorkflowSection(Guid projectId, [FromBody] WorkflowSection section)
        {
            throw new NotImplementedException();
        }
    }
}