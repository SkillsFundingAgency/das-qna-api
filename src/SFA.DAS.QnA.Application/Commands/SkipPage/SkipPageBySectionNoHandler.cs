using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Application.Commands.SkipPage
{
    public class SkipPageBySectionNoHandler : SetAnswersBase, IRequestHandler<SkipPageBySectionNoRequest, HandlerResponse<SkipPageResponse>>
    {
        private readonly QnaDataContext _dataContext;

        public SkipPageBySectionNoHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<SkipPageResponse>> Handle(SkipPageBySectionNoRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.FirstOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken: cancellationToken);
            if (application is null) return new HandlerResponse<SkipPageResponse>(false, "Application does not exist");

            var sequence = await _dataContext.ApplicationSequences.FirstOrDefaultAsync(seq => seq.SequenceNo == request.SequenceNo && seq.ApplicationId == request.ApplicationId, cancellationToken: cancellationToken);
            if (sequence is null) return new HandlerResponse<SkipPageResponse>(false, "Sequence does not exist");

            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.SectionNo == request.SectionNo && sec.SequenceNo == request.SequenceNo && sec.ApplicationId == request.ApplicationId, cancellationToken);
            if (section is null) return new HandlerResponse<SkipPageResponse>(false, "Section does not exist");

            var qnaData = new QnAData(section.QnAData);
            var page = qnaData.Pages.FirstOrDefault(p => p.PageId == request.PageId);
            if (page is null) return new HandlerResponse<SkipPageResponse>(false, "Page does not exist");

            var answers = page.PageOfAnswers.SelectMany(a => a.Answers).ToList();

            try
            {
                var nextAction = GetNextAction(page, answers, section, _dataContext);
                var checkboxListAllNexts = GetCheckboxListMatchingNextActions(page, answers, section, _dataContext);

                SetStatusOfNextPagesBasedOnAnswer(qnaData, page, answers, nextAction, checkboxListAllNexts);

                section.QnAData = qnaData;
                await _dataContext.SaveChangesAsync(cancellationToken);

                return new HandlerResponse<SkipPageResponse>(new SkipPageResponse(nextAction.Action, nextAction.ReturnId));
            }
            catch (ApplicationException)
            {
                if (page.Next is null || !page.Next.Any())
                {
                    return new HandlerResponse<SkipPageResponse>(false, "There are no 'Next' instructions.");
                }
                else
                {
                    return new HandlerResponse<SkipPageResponse>(false, "Cannot find a matching 'Next' instruction");
                }
            }
        }
    }
}
