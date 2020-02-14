using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        ///     Returns the ApplicationData for the Application
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
        ///     Returns the QuestionTag Value for an Application by ApplicationId and QuestionTag
        /// </summary>
        /// <param name="applicationId">ApplicationId (Guid)</param>
        /// <param name="questionTag">QuestionTag (string)</param>
        /// <returns>The QuestionTag Value</returns>
        /// <response code="200">Returns the QuestionTag Value</response>
        /// <response code="404">If there is no Application for the given Application Id or QuestionTag does not exist.</response>
        /// <response code="422">ApplicationData either does not exist or cannot be parsed as json.</response>
        /// <response code="400">ApplicationData is null.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(400)]
        [HttpGet("{applicationId}/applicationData/{questionTag}")]
        public async Task<ActionResult<string>> GetQuestionTag(Guid applicationId, string questionTag)
        {
            var applicationDataResponse = await _mediator.Send(new GetApplicationDataRequest(applicationId));

            if (!applicationDataResponse.Success) return NotFound(new NotFoundError(applicationDataResponse.Message));

            if (applicationDataResponse != null)
            {
                var applicationData = JObject.Parse(applicationDataResponse.Value.ToString());

                if (applicationData != null)
                {
                    var answerData = applicationData[questionTag];
                    if (answerData != null)
                    {
                        return answerData.Value<string>(); 
                    }
                    else
                    {
                        return NotFound(new NotFoundError("QuestionTag does not exist."));
                    }
                }
                else
                {
                    return UnprocessableEntity(new UnprocessableEntityError("ApplicationData either does not exist or cannot be parsed as json."));
                }
            }

            return BadRequest(new BadRequestError("ApplicationData is null"));
        }

        /// <summary>
        ///     Sets the ApplicationData for the Application
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