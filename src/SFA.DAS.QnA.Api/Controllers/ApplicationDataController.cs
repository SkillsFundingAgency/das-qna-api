using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Application.Commands.SetApplicationData;
using SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("/applications")]
    [Produces("application/json")]
    public class ApplicationDataController : Controller
    {
        private readonly IMediator _mediator;

        public ApplicationDataController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Returns the ApplicationData for the Application
        /// </summary>
        /// <returns>The ApplicationData</returns>
        /// <response code="200">Returns the Application's ApplicationData</response>
        /// <response code="404">If there is no Application for the given Application Id</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("{applicationId}/applicationData")]
        public async Task<ActionResult<object>> Get(Guid applicationId)
        {
            var applicationDataResponse = await _mediator.Send(new GetApplicationDataRequest(applicationId));

            if (!applicationDataResponse.Success) return NotFound(new NotFoundError(applicationDataResponse.Message));

            return JsonConvert.DeserializeObject(applicationDataResponse.Value);
        }

        /// <summary>
        /// Sets the ApplicationData for the Application
        /// </summary>
        /// <response code="200">Returns the Application's ApplicationData</response>
        /// <response code="404">If there is no Application for the given Application Id</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpPost("{applicationId}/applicationData")]
        public async Task<ActionResult<object>> Post(Guid applicationId, [FromBody] dynamic applicationData)
        {
            var applicationDataResponse = await _mediator.Send(new SetApplicationDataRequest(applicationId, applicationData));

            if (!applicationDataResponse.Success) return NotFound(new NotFoundError(applicationDataResponse.Message));

            return JsonConvert.DeserializeObject(applicationDataResponse.Value);
        }
    }
}