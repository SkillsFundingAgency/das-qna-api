using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class RequiredValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            if (string.IsNullOrWhiteSpace(answer?.Value))
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(question.QuestionId,
                        ValidationDefinition.ErrorMessage)
                };
            }
            return new List<KeyValuePair<string, string>>();
        }
    }
}