using MediatR;
using SFA.DAS.Qna.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.StartApplication
{
    public class StartApplicationRequest : IRequest<HandlerResponse<StartApplicationResponse>>
    {
        public string UserReference { get; set; }
        public string WorkflowType { get; set; }
    }
}