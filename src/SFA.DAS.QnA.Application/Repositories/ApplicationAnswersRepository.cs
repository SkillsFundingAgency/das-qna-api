using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Repositories
{
    public interface IApplicationAnswersRepository
    {
        Task<List<Answer>> GetPageAnswers(Guid applicationId, Guid sectionId, string pageId);
        Task<List<ApplicationAnswer>> GetSectionAnswers(Guid applicationId, Guid sectionId);
        Task<List<ApplicationAnswer>> GetApplicationAnswers(Guid applicationId);
        Task StoreApplicationAnswers(Guid applicationId, Guid sectionId, string pageId, List<Answer> answers);
    }

    public class ApplicationAnswersRepository : IApplicationAnswersRepository
    {
        private readonly QnaDataContext _dataContext;

        public ApplicationAnswersRepository(QnaDataContext dataContext)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task<List<Answer>> GetPageAnswers(Guid applicationId, Guid sectionId, string pageId)
        {
            return await _dataContext.ApplicationAnswers
                .Where(a => a.ApplicationId == applicationId &&
                            a.SectionId == sectionId && 
                            a.PageId == pageId)
                .Select(a => new Answer
                {
                    QuestionId = a.QuestionId,
                    Value = a.Value
                })
                .ToListAsync();
        }

        public async Task<List<ApplicationAnswer>> GetSectionAnswers(Guid applicationId, Guid sectionId)
        {
            return await _dataContext.ApplicationAnswers
                .Where(a => a.ApplicationId == applicationId &&
                            a.SectionId == sectionId)
                .ToListAsync();
        }

        public async Task<List<ApplicationAnswer>> GetApplicationAnswers(Guid applicationId)
        {
            return await _dataContext.ApplicationAnswers
                .Where(a => a.ApplicationId == applicationId)
                .ToListAsync();
        }

        public async Task StoreApplicationAnswers(Guid applicationId, Guid sectionId, string pageId, List<Answer> answers)
        {
            var deleteSql = $"Delete from ApplicationAnswers where ApplicationId = '{applicationId}' and SectionId = '{sectionId}' and PageId = '{pageId}'";//TODO: use sproc
            await _dataContext.Database.ExecuteSqlCommandAsync(deleteSql);
            await _dataContext.ApplicationAnswers.AddRangeAsync(answers.Select(answer => new ApplicationAnswer{PageId = pageId, SectionId = sectionId, Value = answer.Value, QuestionId = answer.QuestionId, ApplicationId = applicationId}));
        }
    }
}