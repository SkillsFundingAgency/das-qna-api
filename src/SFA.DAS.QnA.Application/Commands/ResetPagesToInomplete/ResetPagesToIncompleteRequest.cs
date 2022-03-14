using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.ResetPagesToInomplete
{
    public class ResetPagesToIncompleteRequest : IRequest<HandlerResponse<bool>>
    { 
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }
        public List<string> PageIdsExcluded { get; }

        public ResetPagesToIncompleteRequest(Guid applicationId, int sequenceNo, int sectionNo, List<string> pageIdsExcluded)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            SectionNo = sectionNo;
            PageIdsExcluded = pageIdsExcluded;
        }
    }
}