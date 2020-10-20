using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Queries.Sections.CanUpdatePage
{
    public class CanUpdatePageHandler : IRequestHandler<CanUpdatePageRequest, HandlerResponse<bool>>
    {
        private readonly QnaDataContext _dataContext;

        public CanUpdatePageHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public async Task<HandlerResponse<bool>> Handle(CanUpdatePageRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.AsNoTracking().SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
            if (application is null) return new HandlerResponse<bool>(false, "Application does not exist");

            var workflowSequences = _dataContext.WorkflowSequences.Where(x => x.WorkflowId == application.WorkflowId);
            var section = _dataContext.WorkflowSections.Single(x => x.Id == request.SectionId);

            var page = section.QnAData.Pages.FirstOrDefault(p => p.PageId == request.PageId);
            if (page is null) return new HandlerResponse<bool>(false, "Page does not exist");
            
            return new HandlerResponse<bool>(page.Active);
        }

        //public async Task<HandlerResponse<bool>> Handle(CanUpdatePageRequest request, CancellationToken cancellationToken)
        //{
        //    var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
        //    if (section is null) return new HandlerResponse<bool>(false, "Section does not exist");

        //    var page = section.QnAData.Pages.FirstOrDefault(p => p.PageId == request.PageId);
        //    if (page is null) return new HandlerResponse<bool>(false, "Page does not exist");

        //    return new HandlerResponse<bool>(page.Active);
        //}

    }
}