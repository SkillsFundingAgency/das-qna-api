using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Application.Queries.Sections.GetNextAction
{
    public class GetNextActionBySectionNoHandler : SetAnswersBase, IRequestHandler<GetNextActionBySectionNoRequest, HandlerResponse<GetNextActionResponse>>
    {
        private readonly QnaDataContext _dataContext;

        public GetNextActionBySectionNoHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<GetNextActionResponse>> Handle(GetNextActionBySectionNoRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.FirstOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken: cancellationToken);
            if (application is null) return new HandlerResponse<GetNextActionResponse>(false, "Application does not exist");

            var sequence = await _dataContext.ApplicationSequences.FirstOrDefaultAsync(seq => seq.SequenceNo == request.SequenceNo && seq.ApplicationId == request.ApplicationId, cancellationToken: cancellationToken);
            if (sequence is null) return new HandlerResponse<GetNextActionResponse>(false, "Sequence does not exist");

            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.SectionNo == request.SectionNo && sec.SequenceNo == request.SequenceNo && sec.ApplicationId == request.ApplicationId, cancellationToken);
            if (section is null) return new HandlerResponse<GetNextActionResponse>(false, "Section does not exist");

            var page = section.QnAData.Pages.FirstOrDefault(p => p.PageId == request.PageId);
            if (page is null) return new HandlerResponse<GetNextActionResponse>(false, "Page does not exist");

            var answers = page.PageOfAnswers.SelectMany(a => a.Answers).ToList();

            try
            {
                var nextAction = GetNextAction(page, answers, section, _dataContext);
                return new HandlerResponse<GetNextActionResponse>(new GetNextActionResponse(nextAction.Action, nextAction.ReturnId));
            }
            catch (ApplicationException)
            {
                if (page.Next is null || !page.Next.Any())
                {
                    return new HandlerResponse<GetNextActionResponse>(false, "There are no 'Next' instructions.");
                }
                else
                {
                    return new HandlerResponse<GetNextActionResponse>(false, "Cannot find a matching 'Next' instruction");
                }
            }
        }
    }
}
