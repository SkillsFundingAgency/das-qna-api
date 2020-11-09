using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequence;
using SFA.DAS.QnA.Application.Repositories;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Services
{
    public class QnADataContextFactory
    {
        private readonly DbContextOptions<QnaDataContext> _options;

        public QnADataContextFactory(DbContextOptions<QnaDataContext> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public QnaDataContext Create() => new QnaDataContext(_options);
    }


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
        private readonly QnADataContextFactory _factory;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationSectionService(QnaDataContext dataContext, IApplicationAnswersRepository answersRepository, 
            INotRequiredProcessor notRequiredProcessor, QnADataContextFactory factory, 
            IWorkflowRepository workflowRepository, IApplicationRepository applicationRepository)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _answersRepository = answersRepository ?? throw new ArgumentNullException(nameof(answersRepository));
            _notRequiredProcessor = notRequiredProcessor ?? throw new ArgumentNullException(nameof(notRequiredProcessor));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
            _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
        }


        public async Task<Section> GetApplicationSection(Guid applicationId, Guid sectionId, CancellationToken cancellationToken)
        {
            var applicationTask = _applicationRepository.GetApplication(applicationId);
            var sectionTask = _workflowRepository.GetWorkflowSection(sectionId);
            var answersDataContext = _factory.Create();
            var answersTask = answersDataContext.ApplicationAnswers
                .AsNoTracking()
                .Where(a => a.ApplicationId == applicationId &&
                            a.SectionId == sectionId)
                .ToListAsync(cancellationToken);
            var sectionStateTask = _applicationRepository.GetApplicationSectionPageStates(applicationId, sectionId);
            Task.WaitAll(new Task[] { applicationTask, answersTask, sectionTask, sectionStateTask });
            //var sequence = sequenceTask.Result;
            var section = sectionTask.Result;
            var answers = answersTask.Result;
            var application = applicationTask.Result;
            var sectionState = sectionStateTask.Result;

            //TODO: Check the results of the db calls

            var applicationSection = new Section
            {
                ApplicationId = application.Id,
                DisplayType = section.DisplayType,
                Id = section.Id,
                LinkTitle = section.LinkTitle,
                QnAData = section.QnAData,
                //SectionNo = sequence.SectionNo,
                //SequenceNo = sequence.SequenceNo,
                Status = "", //todo: how to get status?
                Title = section.Title
            };
            foreach (var page in applicationSection.QnAData.Pages)
            {
                var pageAnswers = answers.Where(pa => pa.PageId == page.PageId)
                    .ToList();
                if (pageAnswers.Any())
                {
                    page.PageOfAnswers =
                        new List<PageOfAnswers>(new[] { new PageOfAnswers { Answers = pageAnswers.Select(pa => new Answer { Value = pa.Value, QuestionId = pa.QuestionId }).ToList() } });
                    page.Complete = true;
                    page.Feedback?.ForEach(feedback => feedback.IsCompleted = true);
                }

                var pageState = sectionState.FirstOrDefault(x => x.PageId == page.PageId);
                if (pageState != null)
                {
                    page.Complete = pageState.Complete;
                    page.Active = pageState.Active;
                    page.NotRequired = pageState.NotRequired;
                }
            }
            RemovePages(application, applicationSection);
            return applicationSection;
        }

        public async Task<List<Section>> GetApplicationSections(Guid applicationId, CancellationToken cancellationToken)
        {
            try
            {
                var application = await _applicationRepository.GetApplication(applicationId);
                if (application is null)
                    //log error here
                    return null;


                var workflowSequences = await _workflowRepository.GetWorkflowSections(application.WorkflowId); //workflowSequencesTask.Result;
                var answers = await _answersRepository.GetApplicationAnswers(applicationId); //answersTask.Result;
                var pageStates = await _applicationRepository.GetApplicationSectionPageStates(applicationId);

                var sections = new List<Section>();
                foreach (var sequence in workflowSequences)
                {
                    var ws = sequence;
                    var section = new Section
                    {
                        ApplicationId = applicationId,
                        DisplayType = ws.DisplayType,
                        Id = ws.Id,
                        LinkTitle = ws.LinkTitle,
                        QnAData = ws.QnAData,
                        //SectionNo = sequence.SectionNo,
                        //SequenceNo = sequence.SequenceNo,
                        Status = "", //todo: how to get status?
                        Title = ws.Title
                    };
                    sections.Add(section);
                    var sectionPagesOfAnswers = answers.Where(a => a.SectionId == ws.Id).ToList();
                    foreach (var page in section.QnAData.Pages)
                    {
                        var pageAnswers = sectionPagesOfAnswers.Where(pa => pa.PageId == page.PageId)
                            .ToList();
                        if (pageAnswers.Any())
                        {
                            page.PageOfAnswers =
                                new List<PageOfAnswers>(new[] { new PageOfAnswers { Answers = pageAnswers.Select(pa => new Answer { Value = pa.Value, QuestionId = pa.QuestionId }).ToList() } });
                            page.Complete = true;
                            page.Feedback?.ForEach(feedback => feedback.IsCompleted = true);
                        }

                        var pageState = pageStates.FirstOrDefault(x => x.PageId == page.PageId);
                        if (pageState != null)
                        {
                            page.Complete = pageState.Complete;
                            page.Active = pageState.Active;
                            page.NotRequired = pageState.NotRequired;
                        }
                    }
                    RemovePages(application, section);
                }
                return sections;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Section>> GetApplicationSections(Guid applicationId, Guid sequenceId, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.GetApplication(applicationId);
            if (application is null)
                //log error here
                return null;

            var workflowSectionsTask = _workflowRepository.GetWorkflowSections(application.WorkflowId, sequenceId);
            var answersTask = _answersRepository.GetApplicationAnswers(applicationId);
            var pageStatesTask = _applicationRepository.GetApplicationSectionPageStates(applicationId);
            Task.WaitAll(new Task[] { workflowSectionsTask, answersTask, pageStatesTask }, cancellationToken);


            var workflowSections = workflowSectionsTask.Result;
            var answers = answersTask.Result;
            var pageStates = pageStatesTask.Result;

            var sections = new List<Section>();
            foreach (var workflowSection in workflowSections)
            {
                var ws = workflowSection;
                var sectionPagesOfAnswers = answers.Where(a => a.SectionId == ws.Id).ToList();
                var section = new Section
                {
                    ApplicationId = applicationId,
                    DisplayType = ws.DisplayType,
                    Id = ws.Id,
                    LinkTitle = ws.LinkTitle,
                    QnAData = ws.QnAData,
                    //SectionNo = sequence.SectionNo,
                    //SequenceNo = sequence.SequenceNo,
                    Status = "", //todo: how to get status?
                    Title = ws.Title
                };
                sections.Add(section);
                foreach (var page in section.QnAData.Pages)
                {
                    var pageAnswers = sectionPagesOfAnswers.Where(pa => pa.PageId == page.PageId)
                        .ToList();
                    if (pageAnswers.Any())
                    {
                        page.PageOfAnswers =
                            new List<PageOfAnswers>(new[] { new PageOfAnswers { Answers = pageAnswers.Select(pa => new Answer { Value = pa.Value, QuestionId = pa.QuestionId }).ToList() } });
                        page.Complete = true;
                        page.Feedback?.ForEach(feedback => feedback.IsCompleted = true);
                    }

                    var pageState = pageStates.FirstOrDefault(x => x.PageId == page.PageId);
                    if (pageState != null)
                    {
                        page.Complete = pageState.Complete;
                        page.Active = pageState.Active;
                        page.NotRequired = pageState.NotRequired;
                    }
                }
                RemovePages(application, section);
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