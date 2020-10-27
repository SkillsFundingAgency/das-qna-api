using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Queries.Sections.GetSections
{
    public class GetSectionsHandler : IRequestHandler<GetSectionsRequest, HandlerResponse<List<Section>>>
    {
        private readonly IApplicationSectionService _applicationSectionService;
        private readonly IMapper _mapper;
        private readonly INotRequiredProcessor _notRequiredProcessor;
        private readonly ILogger<GetSectionsHandler> _logger;

        public GetSectionsHandler(IApplicationSectionService applicationSectionService, IMapper mapper, INotRequiredProcessor notRequiredProcessor, ILogger<GetSectionsHandler> logger)
        {
            _applicationSectionService = applicationSectionService ?? throw new ArgumentNullException(nameof(applicationSectionService));
            _mapper = mapper;
            _notRequiredProcessor = notRequiredProcessor;
            _logger = logger;
        }

        //public async Task<HandlerResponse<List<Section>>> Handle(GetSectionsRequest request, CancellationToken cancellationToken)
        //{
        //    var stopwatch = Stopwatch.StartNew();

        //    var application = await _dataContext.Applications.AsNoTracking().SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
        //    if (application is null) return new HandlerResponse<List<Section>>(false, "Application does not exist");

        //    var sections = await _dataContext.ApplicationSections.AsNoTracking()
        //        .Where(seq => seq.ApplicationId == request.ApplicationId)
        //        .ToListAsync(cancellationToken: cancellationToken);

        //    var mappedSections = _mapper.Map<List<Section>>(sections);

        //    foreach (var section in mappedSections)
        //    {
        //        RemovePages(application, section);
        //    }

        //    _logger.LogInformation($"Original GetSectionsHandler total execution time: {stopwatch.ElapsedMilliseconds} ms");

        //    return new HandlerResponse<List<Section>>(mappedSections);
        //}


        public async Task<HandlerResponse<List<Section>>> Handle(GetSectionsRequest request, CancellationToken cancellationToken)
        {
            var sections = await _applicationSectionService.GetApplicationSections(request.ApplicationId, cancellationToken);

            //var application = await _dataContext.Applications.AsNoTracking().SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
            //if (application is null) return new HandlerResponse<List<Section>>(false, "Application does not exist");


            //var workflowSequences = _dataContext.WorkflowSequences.Where(x => x.WorkflowId == application.WorkflowId);
            //var sectionIds = workflowSequences.Select(x => x.SectionId).ToList();
            //var workflowSections = await _dataContext.WorkflowSections.Where(x => sectionIds.Contains(x.Id)).ToListAsync(cancellationToken);

            //var sections = new List<Section>();
            //foreach (var sequence in workflowSequences)
            //{
            //    foreach (var ws in workflowSections.Where(x => sequence.SectionId == x.Id))
            //    {
            //        sections.Add(new Section
            //        {
            //            ApplicationId = request.ApplicationId,
            //            DisplayType = ws.DisplayType,
            //            Id = ws.Id,
            //            LinkTitle = ws.LinkTitle,
            //            QnAData = ws.QnAData,
            //            SectionNo = sequence.SectionNo,
            //            SequenceNo = sequence.SequenceNo,
            //            Status = "", //todo: how to get status?
            //            Title = ws.Title
            //        });
            //    }
            //}

            //foreach (var section in sections)
            //{
            //    RemovePages(application, section);
            //}


            //var sections = await _dataContext.ApplicationSections.AsNoTracking()
            //    .Where(seq => seq.ApplicationId == request.ApplicationId)
            //    .ToListAsync(cancellationToken: cancellationToken);

            //var mappedSections = _mapper.Map<List<Section>>(sections);

            //foreach (var section in mappedSections)
            //{
            //    RemovePages(application, section);
            //}

            return new HandlerResponse<List<Section>>(sections);
        }

        private void RemovePages(Data.Entities.Application application, Section section)
        {
            var applicationData = JObject.Parse(application.ApplicationData);

            RemovePagesBasedOnNotRequiredConditions(section, applicationData);
            RemoveInactivePages(section);
        }

        private static void RemoveInactivePages(Section section)
        {
            section.QnAData.Pages.RemoveAll(p => !p.Active);
        }

        private void RemovePagesBasedOnNotRequiredConditions(Section section, JObject applicationData)
        {
            section.QnAData.Pages =
                _notRequiredProcessor.PagesWithoutNotRequired(section.QnAData.Pages, applicationData).ToList();

        }
    }
}
