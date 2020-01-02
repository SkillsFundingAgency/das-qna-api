using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Queries.Sections.GetSections
{
    public class GetSectionsHandler : IRequestHandler<GetSectionsRequest, HandlerResponse<List<Section>>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;

        public GetSectionsHandler(QnaDataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<HandlerResponse<List<Section>>> Handle(GetSectionsRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
            if (application is null) return new HandlerResponse<List<Section>>(false, "Application does not exist");

            var sections = await _dataContext.ApplicationSections
                .Where(seq => seq.ApplicationId == request.ApplicationId)
                .ToListAsync(cancellationToken: cancellationToken);

            var mappedSections = _mapper.Map<List<Section>>(sections);

            return new HandlerResponse<List<Section>>(mappedSections);
        }
    }
}
