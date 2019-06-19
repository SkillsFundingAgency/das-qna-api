using MediatR;

namespace SFA.DAS.QnA.Application.Commands.StartApplication
{
    public class StartApplicationRequest : IRequest<StartApplicationResponseBase>
    {
        public string UserReference { get; set; }
        public string Type { get; set; }
    }
}