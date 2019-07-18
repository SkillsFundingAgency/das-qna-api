using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Infrastructure;

namespace SFA.DAS.QnA.Api.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/errors")]
    public class ErrorsController : Controller
    {
        [HttpGet("{code}")]
        public IActionResult Error(int code)
        {
            var parsedCode = (HttpStatusCode) code;
            var error = new ApiError(code, parsedCode.ToString());
            return new ObjectResult(error);
        }
        
        [HttpPost("{code}")]
        public IActionResult ErrorPost(int code)
        {
            var parsedCode = (HttpStatusCode) code;
            var error = new ApiError(code, parsedCode.ToString());
            return new ObjectResult(error);
        }
    }
}