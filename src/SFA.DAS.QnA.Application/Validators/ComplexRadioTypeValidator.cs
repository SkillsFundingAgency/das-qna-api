using System.Collections.Generic;
using System.Linq;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class ComplexRadioTypeValidator : IValidator
    {
        public ComplexRadioTypeValidator()
        {
            ValidationDefinition = new ValidationDefinition() {ErrorMessage = "Answer must be one of the Input Options"};
        }
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var validValues = question.Input.Options.Select(o => o.Value).ToList();
            if (validValues.Any(v => v == answer.Value))
            {
                return new List<KeyValuePair<string, string>>();
            }
            
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(answer.QuestionId,
                    ValidationDefinition.ErrorMessage)
            };
        }
    }
}