using System;
using MediatR;

namespace SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData
{
    public class GetApplicationDataRequest : IRequest<HandlerResponse<string>>
    {
        public Guid ApplicationId { get; }

        public GetApplicationDataRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}