using System;

namespace SFA.DAS.QnA.Application.Commands.StartApplication
{
    public class StartApplicationResponseBase : HandlerResponseBase
    {
        public Guid ApplicationId { get; set; }
    }
}