using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.WorkflowSections.CreateWorkflowSection;
using SFA.DAS.QnA.Application.Commands.WorkflowSections.UpsertWorkflowSection;
using SFA.DAS.QnA.Application.Queries.WorkflowSections.GetWorkflowSection;
using SFA.DAS.QnA.Application.Queries.WorkflowSections.GetWorkflowSections;
using SFA.DAS.QnA.Data.Entities;

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
//            var getWorkflowSectionsResponse = await _mediator.Send(new GetWorkflowSectionsRequest(projectId));
//            if (!getWorkflowSectionsResponse.Success) return NotFound(new NotFoundError(getWorkflowSectionsResponse.Message));
//
//            return getWorkflowSectionsResponse.Value;
throw new NotImplementedException();
        }

        [HttpGet("{projectId}/workflows/{sectionId}")]
        public async Task<ActionResult<WorkflowSection>> GetWorkflowSection(Guid projectId, Guid sectionId)
        {
//            var getWorkflowSectionResponse = await _mediator.Send(new GetWorkflowSectionRequest(projectId, sectionId));
//            if (!getWorkflowSectionResponse.Success) return NotFound(new NotFoundError(getWorkflowSectionResponse.Message));
//
//            return getWorkflowSectionResponse.Value;
            throw new NotImplementedException();
        }

        [HttpPut("{projectId}/workflows/{sectionId}")]
        public async Task<ActionResult<WorkflowSection>> UpsertWorkflowSection(Guid projectId, Guid sectionId, [FromBody] WorkflowSection section)
        {
//            var upsertWorkflowSectionResponse = await _mediator.Send(new UpsertWorkflowSectionRequest(projectId, sectionId, section));
//            if (!upsertWorkflowSectionResponse.Success) return BadRequest(new BadRequestError(upsertWorkflowSectionResponse.Message));
//            
//            return upsertWorkflowSectionResponse.Value;
            throw new NotImplementedException();
        }

        [HttpPost("{projectId}/workflows")]
        public async Task<ActionResult<WorkflowSection>> CreateWorkflowSection(Guid projectId, [FromBody] WorkflowSection section)
        {
//            var createWorkflowSectionResponse = await _mediator.Send(new CreateWorkflowSectionRequest(projectId, section));
//            if (!createWorkflowSectionResponse.Success) return BadRequest(new BadRequestError(createWorkflowSectionResponse.Message));
//            
//            return createWorkflowSectionResponse.Value;
            throw new NotImplementedException();
        }
    }
}