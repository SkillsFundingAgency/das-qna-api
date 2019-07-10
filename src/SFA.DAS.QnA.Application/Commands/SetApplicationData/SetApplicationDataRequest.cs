using System;
using MediatR;

namespace SFA.DAS.QnA.Application.Commands.SetApplicationData
{
    public class SetApplicationDataRequest : IRequest<HandlerResponse<string>>
    {
        public Guid ApplicationId { get; }
        public object ApplicationData { get; }

        public SetApplicationDataRequest(Guid applicationId, object applicationData)
        {
            ApplicationId = applicationId;
            ApplicationData = applicationData;
        }
    }
}