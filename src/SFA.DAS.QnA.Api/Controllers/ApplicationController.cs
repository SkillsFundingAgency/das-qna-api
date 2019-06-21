using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Application.Commands.StartApplication;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("/applications")]
    [Produces("application/json")]
    public class ApplicationController : Controller
    {
        private readonly IMediator _mediator;

        public ApplicationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        /// <summary>
        /// Creates and starts a new application
        /// </summary>
        /// <param name="request">The required parameters to start the application</param>
        /// <returns>The newly created application's Id</returns>
        /// <response code="201">Returns the newly created application's Id</response>
        /// <response code="400">If the WorkflowType does not exist</response>
        [HttpPost("start")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> StartApplication([FromBody] StartApplicationRequest request)
        {
            var newApplication = await _mediator.Send(request);

            if (newApplication.Success == false)
            {
                return BadRequest(new BadRequestError(newApplication.Message));
            }
            
            return CreatedAtAction("GetCurrentSequence", "Sequences", new {newApplication.Value.ApplicationId}, new {newApplication.Value.ApplicationId});
        }
    }
}