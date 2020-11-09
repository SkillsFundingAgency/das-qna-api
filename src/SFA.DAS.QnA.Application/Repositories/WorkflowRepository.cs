using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Repositories
{
    public interface IWorkflowRepository
    {
        Task<Workflow> GetWorkflow(Guid workflowId);
        Task<Workflow> GetWorkflow(string workflowType);
        Task<WorkflowSection> GetWorkflowSection(Guid workflowSectionId);
        Task<WorkflowSection> GetWorkflowSection(Guid workflowId, int sequenceNo, int sectionNo);
        Task<List<WorkflowSection>> GetWorkflowSections(Guid workflowId);
        Task<List<WorkflowSection>> GetWorkflowSections(Guid workflowId, Guid workflowSequenceId);

    }

    public class WorkflowRepository: IWorkflowRepository
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMemoryCache _cache;

        public WorkflowRepository(QnaDataContext dataContext, IMemoryCache memoryCache)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<Workflow> GetWorkflow(Guid workflowId)
        {
            if (!_cache.TryGetValue(CacheKeys.WorkflowByIdKey(workflowId), out Workflow workflow))
            {
                workflow = await _dataContext.Workflows.AsNoTracking().FirstOrDefaultAsync(x => x.Id == workflowId);
                _cache.Set(CacheKeys.WorkflowByIdKey(workflowId), workflow, GetCacheOptions());
                _cache.Set(CacheKeys.WorkflowByTypeKey(workflow.Type), workflow, GetCacheOptions());
            }
            return workflow;
        }

        public async Task<Workflow> GetWorkflow(string workflowType)
        {
            if (!_cache.TryGetValue(CacheKeys.WorkflowByTypeKey(workflowType), out Workflow workflow))
            {
                workflow = await _dataContext.Workflows.AsNoTracking().FirstOrDefaultAsync(x => x.Type == workflowType && x.Status == "Live");
                _cache.Set(CacheKeys.WorkflowByTypeKey(workflowType), workflow, GetCacheOptions());
                _cache.Set(CacheKeys.WorkflowByIdKey(workflow.Id), workflow, GetCacheOptions());
            }
            return workflow;
        }

        public async Task<WorkflowSection> GetWorkflowSection(Guid workflowSectionId)
        {
            if (!_cache.TryGetValue(CacheKeys.WorkflowSectionKey(workflowSectionId),out WorkflowSection workflowSection))
            {
                workflowSection = await  _dataContext.WorkflowSections.AsNoTracking().FirstOrDefaultAsync(x => x.Id == workflowSectionId);
                var options = new MemoryCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(CacheKeys.WorkflowSectionKey(workflowSectionId), workflowSection, options);
            }

            return workflowSection;
        }

        private async Task<List<WorkflowSequence>> GetWorkflowSequences(Guid workflowId)
        {
            if (!_cache.TryGetValue(CacheKeys.WorkflowSequencesKey(workflowId), out List<WorkflowSequence> sequences))
            {
                sequences = await _dataContext.WorkflowSequences
                    .AsNoTracking()
                    .Where(x => x.WorkflowId == workflowId)
                    .ToListAsync();
                _cache.Set(CacheKeys.WorkflowSequencesKey(workflowId), sequences, GetCacheOptions());
            }

            return sequences;
        }

        public async Task<List<WorkflowSection>> GetWorkflowSections(Guid workflowId)
        {
            var workflowSections = new List<WorkflowSection>();
            var sequences = await GetWorkflowSequences(workflowId);
            var workflowSectionIds = sequences.Select(x => x.SectionId).Distinct().ToList();

            var sectionsNotCached = new List<Guid>();
            foreach (var workflowSectionId in workflowSectionIds)
            {
                if (_cache.TryGetValue(CacheKeys.WorkflowSectionKey(workflowSectionId), out WorkflowSection workflowSection))
                {
                    workflowSections.Add(workflowSection);
                }
                else
                    sectionsNotCached.Add(workflowSectionId);
            }

            var sectionIds = sectionsNotCached.ToArray();
            var sections = await _dataContext.WorkflowSections.AsNoTracking().Where(x => sectionIds.Contains(x.Id)).ToListAsync();
            foreach (var workflowSection in sections)
            {
                _cache.Set(CacheKeys.WorkflowSectionKey(workflowSection.Id), workflowSection, GetCacheOptions());
                workflowSections.Add(workflowSection);
            }

            return workflowSections;
        }

        public async Task<List<WorkflowSection>> GetWorkflowSections(Guid workflowId, Guid workflowSequenceId)
        {
            var sequences = await GetWorkflowSequences(workflowId);
            var sequence = sequences.FirstOrDefault(x => x.Id == workflowSequenceId);
            if (sequence == null)
                throw new InvalidOperationException($"Sequence not found. Workflow: {workflowId}, Sequence: {workflowSequenceId}");

            var workflowSectionIds = sequences.Where(x => x.SequenceNo == sequence.SequenceNo).Select(x => x.SectionId).Distinct().ToList();
            var sectionsNotCached = new List<Guid>();
            var workflowSections = new List<WorkflowSection>();
            foreach (var workflowSectionId in workflowSectionIds)
            {
                if (_cache.TryGetValue(CacheKeys.WorkflowSectionKey(workflowSectionId), out WorkflowSection workflowSection))
                {
                    workflowSections.Add(workflowSection);
                }
                else
                    sectionsNotCached.Add(workflowSectionId);
            }

            var sectionIds = sectionsNotCached.ToArray();
            var sections = await _dataContext.WorkflowSections.AsNoTracking().Where(x => sectionIds.Contains(x.Id)).ToListAsync();
            foreach (var workflowSection in sections)
            {
                _cache.Set(CacheKeys.WorkflowSectionKey(workflowSection.Id), workflowSection, GetCacheOptions());
                workflowSections.Add(workflowSection);
            }

            return workflowSections;
        }

        public async Task<List<WorkflowSection>> GetWorkflowSections(Guid workflowId, int workflowSequenceNo)
        {
            var sequences = await GetWorkflowSequences(workflowId);
            var workflowSectionIds = sequences.Where(x => x.SequenceNo  == workflowSequenceNo).Select(x => x.SectionId).Distinct().ToList();
            var sectionsNotCached = new List<Guid>();
            var workflowSections = new List<WorkflowSection>();
            foreach (var workflowSectionId in workflowSectionIds)
            {
                if (_cache.TryGetValue(CacheKeys.WorkflowSectionKey(workflowSectionId), out WorkflowSection workflowSection))
                {
                    workflowSections.Add(workflowSection);
                }
                else
                    sectionsNotCached.Add(workflowSectionId);
            }

            var sectionIds = sectionsNotCached.ToArray();
            var sections = await _dataContext.WorkflowSections.AsNoTracking().Where(x => sectionIds.Contains(x.Id)).ToListAsync();
            foreach (var workflowSection in sections)
            {
                _cache.Set(CacheKeys.WorkflowSectionKey(workflowSection.Id), workflowSection, GetCacheOptions());
                workflowSections.Add(workflowSection);
            }

            return workflowSections;
        }

        public async Task<WorkflowSection> GetWorkflowSection(Guid workflowId, int sequenceNo, int sectionNo)
        {
            var sequences = await GetWorkflowSequences(workflowId);
            var sequence = sequences.FirstOrDefault(x => x.SectionNo == sectionNo && x.SequenceNo == sequenceNo);
            if (sequence == null)
                throw new InvalidOperationException($"Workflow section not found.  Workflow id: {workflowId:D}, Sequence No: {sequenceNo}, Section No: {sectionNo}.");
            return await GetWorkflowSection(sequence.SectionId);
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            var options = new MemoryCacheEntryOptions();
            options.SetSlidingExpiration(TimeSpan.FromMinutes(5));
            options.Priority = CacheItemPriority.Normal;
            return options;
        }
    }
}