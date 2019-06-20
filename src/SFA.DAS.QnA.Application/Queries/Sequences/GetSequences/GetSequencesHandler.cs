using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Qna.Data;

namespace SFA.DAS.QnA.Application.Queries.Sequences.GetSequences
{
    public class GetSequencesHandler : IRequestHandler<GetSequencesRequest, List<SequenceResponse>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;

        public GetSequencesHandler(QnaDataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        
        public async Task<List<SequenceResponse>> Handle(GetSequencesRequest request, CancellationToken cancellationToken)
        {
            var sequences = await _dataContext.ApplicationSequences
                .Where(seq => seq.ApplicationId == request.ApplicationId)
                .ToListAsync(cancellationToken: cancellationToken);

            return _mapper.Map<List<SequenceResponse>>(sequences);
        }
    }
}