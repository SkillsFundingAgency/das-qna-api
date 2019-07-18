using MediatR;
using SFA.DAS.Qna.Api.Types;

namespace SFA.DAS.QnA.Api.Types
{
    public class StartApplicationRequest : IRequest<HandlerResponse<StartApplicationResponse>>
    {
        public string UserReference { get; set; }
        public string WorkflowType { get; set; }

        public string ApplicationData { get; set; }
    }
}