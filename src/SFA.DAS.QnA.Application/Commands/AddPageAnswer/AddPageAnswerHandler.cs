using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Qna.Api.Types;
using SFA.DAS.Qna.Api.Types.Page;
using SFA.DAS.Qna.Data;

namespace SFA.DAS.QnA.Application.Commands.AddPageAnswer
{
    public class AddPageAnswerHandler : PageHandlerBase, IRequestHandler<AddPageAnswerRequest, HandlerResponse<AddPageAnswerResponse>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IAnswerValidator _answerValidator;

        public AddPageAnswerHandler(QnaDataContext dataContext, IAnswerValidator answerValidator) : base(dataContext)
        {
            _dataContext = dataContext;
            _answerValidator = answerValidator;
        }

        public async Task<HandlerResponse<AddPageAnswerResponse>> Handle(AddPageAnswerRequest request, CancellationToken cancellationToken)
        {
            await GetSectionAndPage(request.ApplicationId, request.SectionId, request.PageId);

            Page.PageOfAnswers.Add(new PageOfAnswers() {Id = Guid.NewGuid(), Answers = request.Answers});

            Section.QnAData = QnaData;

            await _dataContext.SaveChangesAsync(CancellationToken.None);

            return new HandlerResponse<AddPageAnswerResponse>();
        }
    }
}