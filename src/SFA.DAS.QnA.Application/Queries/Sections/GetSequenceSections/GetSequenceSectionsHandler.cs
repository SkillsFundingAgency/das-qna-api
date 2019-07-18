using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Qna.Api.Types;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.Qna.Data;

namespace SFA.DAS.QnA.Application.Queries.Sections.GetSequenceSections
{
    public class GetSequenceSectionsHandler : IRequestHandler<GetSequenceSectionsRequest, HandlerResponse<List<Section>>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;

        public GetSequenceSectionsHandler(QnaDataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        
        public async Task<HandlerResponse<List<Section>>> Handle(GetSequenceSectionsRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.FirstOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken: cancellationToken);
            if (application is null) return new HandlerResponse<List<Section>>(false, "Application does not exist");

            var sequence = await _dataContext.ApplicationSequences.FirstOrDefaultAsync(seq => seq.Id == request.SequenceId, cancellationToken: cancellationToken);
            if (sequence is null) return new HandlerResponse<List<Section>>(false, "Sequence does not exist");

            var sections = _mapper.Map<List<Section>>(await _dataContext.ApplicationSections.Where(section => section.SequenceId == request.SequenceId).ToListAsync(cancellationToken: cancellationToken));

            return new HandlerResponse<List<Section>>(sections);
        }
    }
}