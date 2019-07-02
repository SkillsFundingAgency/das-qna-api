using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Qna.Api.Types.Page;
using SFA.DAS.Qna.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.AddPageAnswer
{
    public class PageHandlerBase
    {
        private readonly QnaDataContext _dataContext;
        protected ApplicationSection Section;
        protected QnAData QnaData;
        protected Page Page;

        public PageHandlerBase(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public async Task GetSectionAndPage(Guid applicationId, Guid sectionId, string pageId)
        {
            Section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.Id == sectionId && sec.ApplicationId == applicationId);
            QnaData = new QnAData(Section.QnAData);
            Page = QnaData.Pages.FirstOrDefault(p => p.PageId == pageId);
    
        }
    }
}