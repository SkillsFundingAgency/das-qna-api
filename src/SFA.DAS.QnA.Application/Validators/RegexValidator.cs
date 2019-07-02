using System.Collections.Generic;
using System.Text.RegularExpressions;
using SFA.DAS.Qna.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class RegexValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            if (string.IsNullOrEmpty(answer?.Value)) return new List<KeyValuePair<string, string>>();

            var regex = new Regex(ValidationDefinition.Value.ToString());
            return !regex.IsMatch(answer.Value)
                ? new List<KeyValuePair<string, string>>
                    {new KeyValuePair<string, string>(answer.QuestionId, ValidationDefinition.ErrorMessage)}
                : new List<KeyValuePair<string, string>>();
        }
    }
}