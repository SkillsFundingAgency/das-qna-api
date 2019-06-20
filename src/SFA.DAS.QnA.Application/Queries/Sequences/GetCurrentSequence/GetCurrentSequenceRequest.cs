using System;
using MediatR;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequences;

namespace SFA.DAS.QnA.Application.Queries.Sequences.GetCurrentSequence
{
    public class GetCurrentSequenceRequest : IRequest<SequenceResponse>
    {
        public Guid ApplicationId { get; }

        public GetCurrentSequenceRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}