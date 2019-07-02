using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Qna.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.RemovePageAnswer
{
    public class RemovePageAnswerHandler : IRequestHandler<RemovePageAnswerRequest, HandlerResponse<RemovePageAnswerResponse>>
    {
        public Task<HandlerResponse<RemovePageAnswerResponse>> Handle(RemovePageAnswerRequest request, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}