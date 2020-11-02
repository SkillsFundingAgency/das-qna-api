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

        Task<Section> GetApplicationSection(Guid applicationId, Guid sectionId, CancellationToken cancellationToken);
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

        public async Task<Section> GetApplicationSection(Guid applicationId, Guid sectionId, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.AsNoTracking().FirstOrDefaultAsync(app => app.Id == applicationId, cancellationToken);
            if (application is null)
                //log error here
                return null;

            var workflowSequence = await _dataContext.WorkflowSequences.FirstOrDefaultAsync(sequence=> sequence.SectionId == sectionId);
            if (workflowSequence == null)
                return null;


            var workflowSection = await _dataContext.WorkflowSections.FirstOrDefaultAsync(section => section.Id == sectionId);
            if (workflowSection == null)
                return null;

            var sectionPagesOfAnswers = await _answersRepository.GetSectionAnswers(applicationId, sectionId);

            var applicationSection = new Section
            {
                ApplicationId = application.Id,
                DisplayType = workflowSection.DisplayType,
                Id = workflowSection.Id,
                LinkTitle = workflowSection.LinkTitle,
                QnAData = workflowSection.QnAData,
                SectionNo = workflowSequence.SectionNo,
                SequenceNo = workflowSequence.SequenceNo,
                Status = "", //todo: how to get status?
                Title = workflowSection.Title
            };
            foreach (var page in applicationSection.QnAData.Pages)
            {
                var pageAnswers = sectionPagesOfAnswers.Where(pa => pa.PageId == page.PageId)
                    .ToList();
                if (!pageAnswers.Any())
                    continue;
                page.PageOfAnswers =
                    new List<PageOfAnswers>(new[] { new PageOfAnswers { Answers = pageAnswers.Select(pa => new Answer { Value = pa.Value, QuestionId = pa.QuestionId }).ToList() } });
                page.Complete = true;
                page.Feedback?.ForEach(feedback => feedback.IsCompleted = true);
            }
            RemovePages(application, applicationSection);
            return applicationSection;

        }

        public async Task<List<Section>> GetApplicationSections(Guid applicationId, CancellationToken cancellationToken)
        {
            //var application = await _dataContext.Applications.AsNoTracking().FirstOrDefaultAsync(app => app.Id == applicationId, cancellationToken);
            //if (application is null)
            //    //log error here
            //    return null;

            var workflowId = await _dataContext.Applications
                .Where(x => x.Id == applicationId)
                .Select(x => x.WorkflowId)
                .FirstOrDefaultAsync(cancellationToken);

            if (workflowId == Guid.Empty)
                return null;

            var workflowSequencesTask = _dataContext.WorkflowSequences.Where(wseq => wseq.WorkflowId == workflowId)
                .Join(_dataContext.WorkflowSections,
                    wseq => wseq.SectionId,
                    wsec => wsec.Id,
                    (sequence, section) => new
                    {
                        SequenceNo = sequence.SequenceNo,
                        SectionNo = sequence.SectionNo,
                        Section = section
                    })
                .ToListAsync();
            var answersTask = _answersRepository.GetApplicationAnswers(applicationId);
            Task.WaitAll(new Task[] { workflowSequencesTask, answersTask }, cancellationToken);

            //TODO: add Sequence ID to application section to get rid of this call?
            //var workflowSequences = await _dataContext.WorkflowSequences
            //    .Where(x => x.WorkflowId == application.WorkflowId)
            //    .ToListAsync(cancellationToken);
            //return await GetApplicationSections(application, workflowSequences, cancellationToken);

            var workflowSequences = workflowSequencesTask.Result;
            var answers = answersTask.Result;
            //TODO: get the state for the pages

            var sections = new List<Section>();
            foreach (var sequence in workflowSequences)
            {
                var ws = sequence.Section;
                var sectionPagesOfAnswers = answers.Where(a => a.SectionId == ws.Id).ToList();
                var section = new Section
                {
                    ApplicationId = applicationId,
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
                foreach (var page in section.QnAData.Pages)
                {
                    var pageAnswers = sectionPagesOfAnswers.Where(pa => pa.PageId == page.PageId)
                        .ToList();
                    if (!pageAnswers.Any())
                        continue;
                    page.PageOfAnswers =
                        new List<PageOfAnswers>(new[] { new PageOfAnswers { Answers = pageAnswers.Select(pa => new Answer { Value = pa.Value, QuestionId = pa.QuestionId }).ToList() } });
                    page.Complete = true;
                    page.Feedback?.ForEach(feedback => feedback.IsCompleted = true);
                }
                //RemovePages(application, section);

            }
            return sections;
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
                    var sectionPagesOfAnswers = answers.Where(a => a.SectionId == ws.Id).ToList();

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
                    foreach (var page in section.QnAData.Pages)
                    {
                        var pageAnswers = sectionPagesOfAnswers.Where(pa => pa.PageId == page.PageId)
                            .ToList();
                        if (!pageAnswers.Any())
                            continue;
                        page.PageOfAnswers =
                            new List<PageOfAnswers>(new[] { new PageOfAnswers { Answers = pageAnswers.Select(pa => new Answer{Value = pa.Value, QuestionId = pa.QuestionId}).ToList() } });
                        page.Complete = true;
                        page.Feedback?.ForEach(feedback => feedback.IsCompleted = true);
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