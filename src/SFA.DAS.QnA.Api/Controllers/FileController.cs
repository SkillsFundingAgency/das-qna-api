using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("applications")]
    [Produces("application/json")]
    public class FileController : Controller
    {
        private readonly IMediator _mediator;

        public FileController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        //TODO: Add some methods to allow File upload.  Might be a good start.
    }
}