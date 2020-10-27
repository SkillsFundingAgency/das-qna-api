using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequence;
using SFA.DAS.QnA.Application.Repositories;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Services
{
    public interface IApplicationSectionService
    {
        Task<List<Section>> GetApplicationSections(Guid applicationId, CancellationToken cancellationToken);

        Task<List<Section>> GetApplicationSections(Guid applicationId, Guid sequenceId,
            CancellationToken cancellationToken);
    }

    public class ApplicationSectionService : IApplicationSectionService
    {
        private readonly QnaDataContext _dataContext;
        private readonly IApplicationAnswersRepository _answersRepository;
        private readonly INotRequiredProcessor _notRequiredProcessor;

        public ApplicationSectionService(QnaDataContext dataContext, IApplicationAnswersRepository answersRepository, INotRequiredProcessor notRequiredProcessor)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _answersRepository = answersRepository ?? throw new ArgumentNullException(nameof(answersRepository));
            _notRequiredProcessor = notRequiredProcessor ?? throw new ArgumentNullException(nameof(notRequiredProcessor));
        }

        public async Task<List<Section>> GetApplicationSections(Guid applicationId, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.AsNoTracking().FirstOrDefaultAsync(app => app.Id == applicationId, cancellationToken);
            if (application is null)
                //log error here
                return null;

            //TODO: add Sequence ID to application section to get rid of this call?
            var workflowSequences = await _dataContext.WorkflowSequences
                .Where(x => x.WorkflowId == application.WorkflowId)
                .ToListAsync(cancellationToken);
            return await GetApplicationSections(application, workflowSequences, cancellationToken);
        }

        public async Task<List<Section>> GetApplicationSections(Guid applicationId, Guid sequenceId, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.AsNoTracking().FirstOrDefaultAsync(app => app.Id == applicationId, cancellationToken);
            if (application is null)
                //log error here
                return null;

            //why do we have to query for the sequence number to then get all sequences with same sequence number
            var sequenceNumber = await _dataContext.WorkflowSequences
                .Where(ws => ws.Id == sequenceId)
                .Select(ws => ws.SequenceNo)
                .FirstOrDefaultAsync(cancellationToken);

            var workflowSequences = await _dataContext.WorkflowSequences.Where(ws => ws.SequenceNo == sequenceNumber).ToListAsync(cancellationToken);
            return await GetApplicationSections(application, workflowSequences, cancellationToken);
        }

        private async Task<List<Section>> GetApplicationSections(Data.Entities.Application application,
            List<WorkflowSequence> workflowSequences, CancellationToken cancellationToken)
        {
            var sectionIds = workflowSequences.Select(x => x.SectionId).ToList();
            var workflowSectionsTask = _dataContext.WorkflowSections.Where(x => sectionIds.Contains(x.Id)).ToListAsync(cancellationToken);
            var answersTask = _answersRepository.GetApplicationAnswers(application.Id);
            Task.WaitAll(new Task[] { workflowSectionsTask, answersTask }, cancellationToken);

            var workflowSections = workflowSectionsTask.Result;
            var answers = answersTask.Result;
            //TODO: get the state for the pages

            var sections = new List<Section>();
            foreach (var sequence in workflowSequences)
            {
                foreach (var ws in workflowSections.Where(x => sequence.SectionId == x.Id))
                {
                    var sectionPagesOfAnswers = answers.FirstOrDefault(a => a.SectionId == ws.Id);
                    var section = new Section
                    {
                        ApplicationId = application.Id,
                        DisplayType = ws.DisplayType,
                        Id = ws.Id,
                        LinkTitle = ws.LinkTitle,
                        QnAData = ws.QnAData,
                        SectionNo = sequence.SectionNo,
                        SequenceNo = sequence.SequenceNo,
                        Status = "", //todo: how to get status?
                        Title = ws.Title
                    };
                    sections.Add(section);
                    if (sectionPagesOfAnswers.SectionId != Guid.Empty)
                    {
                        foreach (var page in section.QnAData.Pages)
                        {
                            var pageAnswers = sectionPagesOfAnswers.Pages.Where(pa => pa.PageId == page.PageId)
                                .SelectMany(pa => pa.Answers).ToList();
                            if (!pageAnswers.Any())
                                continue;
                            page.PageOfAnswers =
                                new List<PageOfAnswers>(new[] { new PageOfAnswers { Answers = pageAnswers } });
                            page.Complete = true;
                            page.Feedback?.ForEach(feedback => feedback.IsCompleted = true);
                        }
                    }
                    RemovePages(application, section);
                }
            }
            return sections;

        }

        private void RemovePages(Data.Entities.Application application, Section section)
        {
            var applicationData = JObject.Parse(application.ApplicationData);  //Dong this for every section?!?!

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