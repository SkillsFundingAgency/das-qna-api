using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Qna.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.WorkflowSections.UpsertWorkflowSection
{
    public class UpsertWorkflowSectionHandler : IRequestHandler<UpsertWorkflowSectionRequest, HandlerResponse<WorkflowSection>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;

        public UpsertWorkflowSectionHandler(QnaDataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        
        public async Task<HandlerResponse<WorkflowSection>> Handle(UpsertWorkflowSectionRequest request, CancellationToken cancellationToken)
        {
            var existingSection = await _dataContext.WorkflowSections.SingleOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ProjectId == request.ProjectId, cancellationToken: cancellationToken);
            if (existingSection == null)
            {
                await _dataContext.WorkflowSections.AddAsync(request.Section, cancellationToken);
            }
            else
            {
                existingSection = _mapper.Map<WorkflowSection>(request.Section);
            }

            await _dataContext.SaveChangesAsync(cancellationToken);
            
            return new HandlerResponse<WorkflowSection>(existingSection);
        }
    }
}