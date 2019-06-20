using System;
using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.QnA.Application.Queries.Sequences.GetSequences
{
    public class GetSequencesRequest : IRequest<List<SequenceResponse>>
    {
        public Guid ApplicationId { get; }

        public GetSequencesRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}