using System;

namespace SFA.DAS.QnA.Application.Commands.StartApplication
{
    public class StartApplicationResponse : HandlerResponseBase
    {
        public Guid ApplicationId { get; set; }
    }
}