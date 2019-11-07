using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SFA.DAS.QnA.Application.Validators
{
    public class NumberTypeValidator : IValidator
    {
        public NumberTypeValidator()
        {
            ValidationDefinition = new ValidationDefinition() {ErrorMessage = "Answer must be a valid number"};
        }

        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            var text = answer?.Value?[0].Trim();

            if (!string.IsNullOrEmpty(text) && !IsValidNumber(text))
            {
                errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
            }

            return errors;
        }

        private static bool IsValidNumber(string number)
        {
            return long.TryParse(number, out var _);
        }
    }
}