using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.Qna.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.PageFeedback.UpsertFeedback;
using SFA.DAS.QnA.Application.Queries.Sections.GetPage;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("applications")]
    [Produces("application/json")]
    public class FeedbackController : Controller
    {
        private readonly IMediator _mediator;

        public FeedbackController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        /// <summary>
        /// Inserts / Updates Page Feedback
        /// </summary>
        /// <returns>The page containing the feedback</returns>
        /// <response code="200">Returns a Page</response>
        /// <response code="404">If the ApplicationId, SectionId or PageId are invalid</response>
        [HttpPut("{applicationId}/sections/{sectionId}/pages/{pageId}/feedback")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Page>> UpsertFeedback(Guid applicationId, Guid sectionId, string pageId, [FromBody] Feedback feedback)
        {
            var upsertFeedbackResponse = await _mediator.Send(new UpsertFeedbackRequest(applicationId, sectionId, pageId, feedback), CancellationToken.None);
            if (!upsertFeedbackResponse.Success) return BadRequest(new BadRequestError(upsertFeedbackResponse.Message));

            return upsertFeedbackResponse.Value;
        }
    }
}