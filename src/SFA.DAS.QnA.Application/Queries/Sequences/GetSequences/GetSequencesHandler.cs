using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Qna.Api.Types;
using SFA.DAS.Qna.Data;

namespace SFA.DAS.QnA.Application.Queries.Sequences.GetSequences
{
    public class GetSequencesHandler : IRequestHandler<GetSequencesRequest, HandlerResponse<List<Sequence>>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;

        public GetSequencesHandler(QnaDataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        
        public async Task<HandlerResponse<List<Sequence>>> Handle(GetSequencesRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
            if (application is null) return new HandlerResponse<List<Sequence>>(false, "Application does not exist");

            var sequences = await _dataContext.ApplicationSequences
                .Where(seq => seq.ApplicationId == request.ApplicationId)
                .ToListAsync(cancellationToken: cancellationToken);

            var mappedSequences = _mapper.Map<List<Sequence>>(sequences);
            
            return new HandlerResponse<List<Sequence>>(mappedSequences);
        }
    }
}