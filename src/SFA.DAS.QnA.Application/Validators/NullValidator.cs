using System.Collections.Generic;
using SFA.DAS.Qna.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class NullValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            return new List<KeyValuePair<string, string>>();
        }
    }
}