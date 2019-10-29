using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Application.Commands.GetNextAction
{
    public class GetNextPageHandler : SetAnswersBase, IRequestHandler<GetNextActionRequest, HandlerResponse<GetNextActionResponse>>
    {
        private readonly QnaDataContext _dataContext;

        public GetNextPageHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<GetNextActionResponse>> Handle(GetNextActionRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            var page = section?.QnAData.Pages.FirstOrDefault(p => p.PageId == request.PageId);
            
            if(section is null || page is null)
            {
                return new HandlerResponse<GetNextActionResponse>(success: false, message: $"Requested page has not been found");
            }

            var answers = page.PageOfAnswers.SelectMany(a => a.Answers).ToList();
            var nextAction = GetNextAction(page, answers, section, _dataContext);

            return new HandlerResponse<GetNextActionResponse>(new GetNextActionResponse(nextAction.Action, nextAction.ReturnId));
        }
    }
}
