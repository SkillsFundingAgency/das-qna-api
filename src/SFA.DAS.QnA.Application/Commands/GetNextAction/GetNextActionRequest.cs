using MediatR;
using SFA.DAS.QnA.Api.Types;
using System;

namespace SFA.DAS.QnA.Application.Commands.GetNextAction
{
    public class GetNextActionRequest : IRequest<HandlerResponse<GetNextActionResponse>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }
        public string PageId { get; }

        public GetNextActionRequest(Guid applicationId, Guid sectionId, string pageId)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
            PageId = pageId;
        }
    }
}
