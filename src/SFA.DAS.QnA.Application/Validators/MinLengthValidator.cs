using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class MinLengthValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            if (answer.Value.Length < (long)ValidationDefinition.Value)
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(answer.QuestionId,
                        ValidationDefinition.ErrorMessage)
                };
            }
            return new List<KeyValuePair<string, string>>();
        }
    }
}