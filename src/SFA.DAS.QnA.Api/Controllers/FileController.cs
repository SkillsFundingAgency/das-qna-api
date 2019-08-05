using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Application.Commands.UploadFile;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("applications")]
    [Produces("application/json")]
    public class FileController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _contextAccessor;

        public FileController(IMediator mediator, IHttpContextAccessor contextAccessor)
        {
            _mediator = mediator;
            _contextAccessor = contextAccessor;
        }

        //TODO: Add some methods to allow File upload.  Might be a good start.
        [HttpPost("{applicationId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}")]
        public async Task<IActionResult> Upload(Guid applicationId, Guid sectionId, string pageId, string questionId)
        {
            var uploadResult = await _mediator.Send(new UploadFileRequest(applicationId, sectionId, pageId, questionId, _contextAccessor.HttpContext.Request.Form.Files));

            if (!uploadResult.Success)
            {
                return BadRequest(new BadRequestError(uploadResult.Message));
            }
            
            return Ok();
        }
    }
}