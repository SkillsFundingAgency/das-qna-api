using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.AddPageAnswer;
using SFA.DAS.QnA.Application.Commands.RemovePageAnswer;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Application.Queries.Sections.GetPage;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("applications")]
    [Produces("application/json")]
    public class PagesController : Controller
    {
        private readonly IMediator _mediator;

        public PagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     Returns the requested Page
        /// </summary>
        /// <returns>The requested Page</returns>
        /// <response code="200">Returns a Page</response>
        /// <response code="404">If the ApplicationId, SectionId or PageId are invalid</response>
        [HttpGet("{applicationId}/sections/{sectionId}/pages/{pageId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Page>> GetPage(Guid applicationId, Guid sectionId, string pageId)
        {
            var sectionsResponse = await _mediator.Send(new GetPageRequest(applicationId, sectionId, pageId), CancellationToken.None);
            if (!sectionsResponse.Success) return NotFound();

            return sectionsResponse.Value;
        }

        /// <summary>
        ///     Sets the answers on the page.
        /// </summary>
        /// <returns>An object describing validity / next steps</returns>
        /// <response code="200">Returns the response</response>
        [HttpPost("{applicationId}/sections/{sectionId}/pages/{pageId}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<SetPageAnswersResponse>> SetPageAnswers(Guid applicationId, Guid sectionId, string pageId, [FromBody] List<Answer> answers)
        {
            var savePageAnswersResponse = await _mediator.Send(new SetPageAnswersRequest(applicationId, sectionId, pageId, answers), CancellationToken.None);

            return savePageAnswersResponse.Value;
        }

        /// <summary>
        ///     Adds an answer on a page that allows multiple sets of answers
        /// </summary>
        /// <returns>An object describing validity</returns>
        /// <response code="200">Returns the Page</response>
        /// <response code="400">If this page is not a multiple answers page or this ApplicationId, SectionId or PageId does not exist</response>
        [HttpPost("{applicationId}/sections/{sectionId}/pages/{pageId}/multiple")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Page>> AddPageAnswer(Guid applicationId, Guid sectionId, string pageId, [FromBody] List<Answer> answers)
        {
            var addPageAnswerResponse = await _mediator.Send(new AddPageAnswerRequest(applicationId, sectionId, pageId, answers), CancellationToken.None);
            if (!addPageAnswerResponse.Success) return BadRequest(new BadRequestError(addPageAnswerResponse.Message));

            return addPageAnswerResponse.Value.Page;
        }

        /// <summary>
        ///     Removes an answer from a page that allows multiple sets of answers
        /// </summary>
        /// <returns>An object describing validity</returns>
        /// <response code="200">Returns the Page</response>
        /// <response code="400">If this page is not a multiple answers page or this ApplicationId, SectionId or PageId does not exist</response>
        [HttpDelete("{applicationId}/sections/{sectionId}/pages/{pageId}/multiple/{answerId}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Page>> RemovePageAnswer(Guid applicationId, Guid sectionId, string pageId, Guid answerId)
        {
            var removePageAnswerResponse = await _mediator.Send(new RemovePageAnswerRequest(applicationId, sectionId, pageId, answerId), CancellationToken.None);
            if (!removePageAnswerResponse.Success) return BadRequest(new BadRequestError(removePageAnswerResponse.Message));

            return removePageAnswerResponse.Value.Page;
        }
    }
}