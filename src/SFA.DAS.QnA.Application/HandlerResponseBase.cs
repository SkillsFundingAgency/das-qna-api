namespace SFA.DAS.QnA.Application
{
    public class HandlerResponseBase
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
    }
}