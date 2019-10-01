using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Queries.Sections.GetSection
{
    public class GetSectionHandler : IRequestHandler<GetSectionRequest, HandlerResponse<Section>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;

        public GetSectionHandler(QnaDataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        
        public async Task<HandlerResponse<Section>> Handle(GetSectionRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.FirstOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken: cancellationToken);
            if (application is null) return new HandlerResponse<Section>(false, "Application does not exist");

            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            if (section is null) return new HandlerResponse<Section>(false, "Section does not exist");

            RemovePagesBasedOnNotRequiredConditions(application, section);

            return new HandlerResponse<Section>(_mapper.Map<Section>(section));
        }

        private static void RemovePagesBasedOnNotRequiredConditions(Data.Entities.Application application, ApplicationSection section)
        {
            var applicationData = JObject.Parse(application.ApplicationData);

            section.QnAData.Pages.RemoveAll(p => p.NotRequiredConditions != null && p.NotRequiredConditions.Any(nrc => nrc.IsOneOf.Contains(applicationData[nrc.Field].Value<string>())));
        }
    }
}