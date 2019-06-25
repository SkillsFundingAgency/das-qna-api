using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Qna.Api.Types;
using SFA.DAS.Qna.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SavePageAnswers;
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
        /// Returns the requested Page
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
        /// Sets the answers on the page.
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
    }
}