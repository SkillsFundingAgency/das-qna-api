using MediatR;

namespace SFA.DAS.QnA.Application.Commands.StartApplication
{
    public class StartApplicationRequest : IRequest<StartApplicationResponse>
    {
        public string UserReference { get; set; }
        public string WorkflowType { get; set; }
    }
}