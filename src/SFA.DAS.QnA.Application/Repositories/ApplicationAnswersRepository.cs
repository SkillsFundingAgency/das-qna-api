using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Repositories
{
    public interface IApplicationAnswersRepository
    {
        Task<List<Answer>> GetPageAnswers(Guid applicationId, Guid sectionId, string pageId);
        Task<List<(string PageId, List<Answer> Answers)>> GetSectionAnswers(Guid applicationId, Guid sectionId);
        Task<List<(Guid SectionId, List<(string PageId, List<Answer> Answers)> Pages)>> GetApplicationAnswers(Guid applicationId);
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
            return new List<Answer>();
        }

        public async Task<List<(string PageId, List<Answer> Answers)>> GetSectionAnswers(Guid applicationId, Guid sectionId)
        {
            return new List<(string PageId, List<Answer> Answers)>();
        }

        public async Task<List<(Guid SectionId, List<(string PageId, List<Answer> Answers)> Pages)>> GetApplicationAnswers(Guid applicationId)
        {
            return new List<(Guid SectionId, List<(string PageId, List<Answer> Answers)> Pages)>();
        }
    }
}