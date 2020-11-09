using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Repositories
{
    public interface IApplicationRepository
    {
        Task<Data.Entities.Application> GetApplication(Guid applicationId);
        Task<ApplicationSection> GetApplicationSection(Guid applicationId, int sequenceNo, int sectionNo);
        Task<List<ApplicationSection>> GetApplicationSections(Guid applicationId);
        Task<List<ApplicationPageState>> GetApplicationSectionPageStates(Guid applicationId);
        Task<List<ApplicationPageState>> GetApplicationSectionPageStates(Guid applicationId, Guid sectionId);
    }

    public class ApplicationRepository : IApplicationRepository
    {
        private readonly QnaDataContext _qnaDataContext;
        private readonly IMemoryCache _cache;

        public ApplicationRepository(QnaDataContext qnaDataContext, IMemoryCache cache)
        {
            _qnaDataContext = qnaDataContext ?? throw new ArgumentNullException(nameof(qnaDataContext));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<Data.Entities.Application> GetApplication(Guid applicationId)
        {
            if (!_cache.TryGetValue(CacheKeys.ApplicationKey(applicationId), out Data.Entities.Application application))
            {
                application = await _qnaDataContext.Applications
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == applicationId);
                _cache.Set(CacheKeys.ApplicationKey(applicationId), application, GetCacheOptions());
            }

            return application;
        }

        public async Task<ApplicationSection> GetApplicationSection(Guid applicationId, int sequenceNo, int sectionNo)
        {
            return await _qnaDataContext.ApplicationSections
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ApplicationId == applicationId &&
                                                         x.SequenceNo == sequenceNo &&
                                                         x.SectionNo == sectionNo);
        }

        public async Task<List<ApplicationSection>> GetApplicationSections(Guid applicationId)
        {
            return await _qnaDataContext.ApplicationSections
                .AsNoTracking()
                .Where(x => x.ApplicationId == applicationId)
                .ToListAsync();
        }

        public async Task<List<ApplicationPageState>> GetApplicationSectionPageStates(Guid applicationId)
        {
            return await _qnaDataContext.ApplicationPageStates
                .AsNoTracking()
                .Where(x => x.ApplicationId == applicationId)
                .ToListAsync();
        }
        public async Task<List<ApplicationPageState>> GetApplicationSectionPageStates(Guid applicationId, Guid sectionId)
        {
            return await _qnaDataContext.ApplicationPageStates
                .AsNoTracking()
                .Where(x => x.ApplicationId == applicationId &&
                            x.SectionId == sectionId)
                .ToListAsync();
        }

        public async Task StoreApplicationSectionPageStates(Guid applicationId, Guid sectionId, List<Page> pages, bool saveChanges = true)
        {
            var deleteSql = $"delete from ApplicationPageStates where ApplicationId='{applicationId}' and SectionId='{sectionId}'";
            await _qnaDataContext.Database.ExecuteSqlCommandAsync(deleteSql);
            await _qnaDataContext.ApplicationPageStates.AddRangeAsync(pages.Select(page => new ApplicationPageState
            {
                SectionId = sectionId,
                PageId = page.PageId,
                ApplicationId = applicationId,
                Complete = page.Complete,
                Active = page.Active,
                NotRequired = page.NotRequired
            }));
            if (saveChanges)
                await _qnaDataContext.SaveChangesAsync();
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            var options = new MemoryCacheEntryOptions();
            options.SetSlidingExpiration(TimeSpan.FromMinutes(5));
            options.Priority = CacheItemPriority.Low;
            return options;
        }
    }
}