using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Application.Queries.WorkflowSections.GetWorkflowSection;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Api.Controllers.Config
{
    [Route("/config/project")]
    public class WorkflowSectionsController : Controller
    {
        private readonly IMediator _mediator;

        public WorkflowSectionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{projectId}/sections")]
        public async Task<ActionResult<List<WorkflowSection>>> GetWorkflowSections(Guid projectId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{projectId}/sections/{sectionId}")]
        public async Task<ActionResult<WorkflowSection>> GetWorkflowSection(Guid projectId, Guid sectionId)
        {
            var getWorkflowSectionResponse = await _mediator.Send(new GetWorkflowSectionRequest(projectId, sectionId));
            if (!getWorkflowSectionResponse.Success) return NotFound(new NotFoundError(getWorkflowSectionResponse.Message));

            return getWorkflowSectionResponse.Value;
        }
        
        [HttpPut("{projectId}/sections/{sectionId}")]
        public async Task<ActionResult<List<WorkflowSection>>> UpdateWorkflowSection(Guid projectId, Guid sectionId, [FromBody] WorkflowSection section)
        {
            throw new NotImplementedException();
        }
        
        [HttpPost("{projectId}/sections")]
        public async Task<ActionResult<List<WorkflowSection>>> CreateWorkflowSection(Guid projectId, [FromBody] WorkflowSection section)
        {
            throw new NotImplementedException();
        }
    }
}