namespace SFA.DAS.Qna.Api.Types
{
    public class RemovePageAnswerResponse
    {
        public Page.Page Page { get; }

        public RemovePageAnswerResponse(Page.Page page)
        {
            Page = page;
        }
    }
}