using System.Collections.Generic;
using SFA.DAS.Qna.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;

namespace SFA.DAS.QnA.Application.Commands
{
    public interface IAnswerValidator
    {
        List<KeyValuePair<string, string>> Validate(SetPageAnswersRequest request, Page page);
    }
}