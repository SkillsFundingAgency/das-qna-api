using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Qna.Api.Types;
using SFA.DAS.Qna.Data;

namespace SFA.DAS.QnA.Application.Queries.Sequences.GetSequence
{
    public class GetSequenceHandler : IRequestHandler<GetSequenceRequest, HandlerResponse<Sequence>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;

        public GetSequenceHandler(QnaDataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        public async Task<HandlerResponse<Sequence>> Handle(GetSequenceRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.FirstOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken: cancellationToken);
            if (application is null) return new HandlerResponse<Sequence>(false, "Application does not exist");

            var sequence = await _dataContext.ApplicationSequences.FirstOrDefaultAsync(seq => seq.Id == request.SequenceId, cancellationToken: cancellationToken);
            if (sequence is null) return new HandlerResponse<Sequence>(false, "Sequence does not exist");

            return new HandlerResponse<Sequence>(_mapper.Map<Sequence>(sequence));
        }
    }
}