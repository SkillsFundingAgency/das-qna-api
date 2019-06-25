using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Qna.Api.Types;
using SFA.DAS.Qna.Api.Types.Page;
using SFA.DAS.Qna.Data;
using SFA.DAS.QnA.Application.Queries.Sections.GetPage;

namespace SFA.DAS.QnA.Application.Commands.SavePageAnswers
{
    public class SetPageAnswersHandler : IRequestHandler<SetPageAnswersRequest, HandlerResponse<SetPageAnswersResponse>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMediator _mediator;

        public SetPageAnswersHandler(QnaDataContext dataContext, IMediator mediator)
        {
            _dataContext = dataContext;
            _mediator = mediator;
        }
        
        public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(SetPageAnswersRequest request, CancellationToken cancellationToken)
        {
            var getPageResponse = await _mediator.Send(new GetPageRequest(request.ApplicationId, request.SectionId, request.PageId), cancellationToken);
            if (!getPageResponse.Success) return new HandlerResponse<SetPageAnswersResponse>(success:false, message:getPageResponse.Message);

            var page = getPageResponse.Value;
            
            if (page.AllowMultipleAnswers) return new HandlerResponse<SetPageAnswersResponse>(success:false, message:"This endpoint cannot be used for Multiple Answers pages. Use AddAnswer / RemoveAnswer instead.");

            return null;
        }
    }
}