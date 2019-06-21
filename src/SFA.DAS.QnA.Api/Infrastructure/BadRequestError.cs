using System.Net;

namespace SFA.DAS.QnA.Api.Infrastructure
{
    public class BadRequestError : ApiError
    {
        public BadRequestError()
            : base(400, HttpStatusCode.InternalServerError.ToString())
        {
        }


        public BadRequestError(string message)
            : base(400, HttpStatusCode.InternalServerError.ToString(), message)
        {
        }
    }
}