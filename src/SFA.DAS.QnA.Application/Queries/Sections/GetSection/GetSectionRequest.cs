using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.Qna.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.Sections.GetSection
{
    public class GetSectionRequest : IRequest<HandlerResponse<Section>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }

        public GetSectionRequest(Guid applicationId, Guid sectionId)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
        }
    }
}