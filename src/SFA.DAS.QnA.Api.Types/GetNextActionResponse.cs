namespace SFA.DAS.QnA.Api.Types
{
    public class GetNextActionResponse
    {
        public string NextAction { get; set; }

        public string NextActionId { get; set; }

        public GetNextActionResponse()
        { }
        
        public GetNextActionResponse(string nextAction, string nextActionId)
        {
            NextAction = nextAction;
            NextActionId = nextActionId;
        }
    }
}