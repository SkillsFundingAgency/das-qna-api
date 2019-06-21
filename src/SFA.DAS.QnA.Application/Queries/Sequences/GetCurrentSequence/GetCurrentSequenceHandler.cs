using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Qna.Api.Types;
using SFA.DAS.Qna.Data;

namespace SFA.DAS.QnA.Application.Queries.Sequences.GetCurrentSequence
{
    public class GetCurrentSequenceHandler : IRequestHandler<GetCurrentSequenceRequest, HandlerResponse<Sequence>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;

        public GetCurrentSequenceHandler(QnaDataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        
        public async Task<HandlerResponse<Sequence>> Handle(GetCurrentSequenceRequest request, CancellationToken cancellationToken)
        {
            var currentSequence = await _dataContext.ApplicationSequences.FirstOrDefaultAsync(seq => seq.ApplicationId == request.ApplicationId && seq.IsActive, cancellationToken);

            return new HandlerResponse<Sequence>(_mapper.Map<Sequence>(currentSequence));  
        }
    }
}