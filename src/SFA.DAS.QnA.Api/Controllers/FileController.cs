using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.Files.DeleteFile;
using SFA.DAS.QnA.Application.Commands.Files.DownloadFile;
using SFA.DAS.QnA.Application.Commands.Files.UploadFile;

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
//
//        [HttpPost("{applicationId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/upload")]
//        public async Task<IActionResult> Upload(Guid applicationId, Guid sectionId, string pageId, string questionId)
//        {
//            var uploadResult = await _mediator.Send(new UploadFileRequest(applicationId, sectionId, pageId, questionId, _contextAccessor.HttpContext.Request.Form.Files));
//
//            if (!uploadResult.Success)
//            {
//                return BadRequest(new BadRequestError(uploadResult.Message));
//            }
//            
//            return Ok();
//        }
        
        [HttpPost("{applicationId}/sections/{sectionId}/pages/{pageId}/upload")]
        public async Task<ActionResult<SetPageAnswersResponse>> Upload(Guid applicationId, Guid sectionId, string pageId)
        {
            var uploadResult = await _mediator.Send(new SubmitPageOfFilesRequest(applicationId, sectionId, pageId, _contextAccessor.HttpContext.Request.Form.Files));

            if (!uploadResult.Success)
            {
                return BadRequest(new BadRequestError(uploadResult.Message));
            }
            
            return uploadResult.Value;
        } 
        
        [HttpGet("{applicationId}/sections/{sectionId}/pages/{pageId}/download")]
        public async Task<IActionResult> DownloadPageZipOfFiles(Guid applicationId, Guid sectionId, string pageId)
        {
            var downloadResult = await _mediator.Send(new DownloadFileRequest(applicationId, sectionId, pageId,null, null));

            if (!downloadResult.Success)
            {
                return BadRequest(new BadRequestError(downloadResult.Message));
            }

            var downloadResultValue = downloadResult.Value;
            
            return File(downloadResultValue.Stream, downloadResultValue.ContentType, downloadResultValue.FileName);
        }
        
        [HttpGet("{applicationId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/download")]
        public async Task<IActionResult> DownloadFileOrZipOfFiles(Guid applicationId, Guid sectionId, string pageId, string questionId)
        {
            var downloadResult = await _mediator.Send(new DownloadFileRequest(applicationId, sectionId, pageId, questionId, null));

            if (!downloadResult.Success)
            {
                return BadRequest(new BadRequestError(downloadResult.Message));
            }

            var downloadResultValue = downloadResult.Value;
            
            return File(downloadResultValue.Stream, downloadResultValue.ContentType, downloadResultValue.FileName);
        }
        
        [HttpGet("{applicationId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/download/{fileName}")]
        public async Task<IActionResult> DownloadFile(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName)
        {
            var downloadResult = await _mediator.Send(new DownloadFileRequest(applicationId, sectionId, pageId, questionId, fileName));

            if (!downloadResult.Success)
            {
                return BadRequest(new BadRequestError(downloadResult.Message));
            }

            var downloadResultValue = downloadResult.Value;
            
            return File(downloadResultValue.Stream, downloadResultValue.ContentType, downloadResultValue.FileName);
        }

        [HttpDelete("{applicationId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/download/{fileName}")]
        public async Task<IActionResult> DeleteFile(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName)
        {
            var deleteFileResponse = await _mediator.Send(new DeleteFileRequest(applicationId, sectionId, pageId, questionId, fileName));

            if (!deleteFileResponse.Success)
            {
                return BadRequest(new BadRequestError(deleteFileResponse.Message));
            }

            return Ok();
        }
    }
}