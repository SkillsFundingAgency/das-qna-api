using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

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

            foreach (var section in sections)
            {
                RemovePages(application, section);
            }
            
            return new HandlerResponse<List<Section>>(sections);
        }
        
        private static void RemovePages(Data.Entities.Application application, Section section)
        {
            var applicationData = JObject.Parse(application.ApplicationData);

            RemovePagesBasedOnNotRequiredConditions(section, applicationData);
            RemoveInactivePages(section);
        }

        private static void RemoveInactivePages(Section section)
        {
            section.QnAData.Pages.RemoveAll(p => !p.Active);
        }

        private static void RemovePagesBasedOnNotRequiredConditions(Section section, JObject applicationData)
        {
            section.QnAData.Pages.RemoveAll(p => p.NotRequiredConditions != null && p.NotRequiredConditions.Any(nrc => nrc.IsOneOf.Contains(applicationData[nrc.Field]?.Value<string>())));
        }
    }
}