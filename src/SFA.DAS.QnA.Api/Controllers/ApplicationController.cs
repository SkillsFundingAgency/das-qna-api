using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Application.Commands.StartApplication;

namespace SFA.DAS.QnA.Api.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly IMediator _mediator;

        public ApplicationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost("/Application/Start")]
        public async Task<ActionResult> Start([FromBody] StartApplicationRequest request)
        {
            var newApplication = await _mediator.Send(request);
            
            return CreatedAtAction("Application", "Application", new {newApplication.ApplicationId}, new {newApplication.ApplicationId});
        }

        [HttpGet("/Application/{applicationId}")]
        public async Task<ActionResult> Application(Guid applicationId)
        {
            return Ok(new {ApplicationId = applicationId});
        }
    }
}