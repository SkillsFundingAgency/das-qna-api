using System.Collections.Generic;
using System.Linq;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class ComplexRadioTypeValidator : IValidator
    {
        private ValidationDefinition _validationDefinition;
       
        public ValidationDefinition ValidationDefinition
        {
            get
            {
                if (_validationDefinition == null)
                {
                    _validationDefinition = new ValidationDefinition() { ErrorMessage = "Answer must be one of the Input Options" };
                }
                return _validationDefinition;
            }
            set
            {
                _validationDefinition = value;
            }
        }

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