using System.Collections.Generic;
using SFA.DAS.Qna.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Commands.SetPageAnswers
{
    public interface IAnswerValidator
    {
        List<KeyValuePair<string, string>> Validate(SetPageAnswersRequest request, Page page);
    }
}