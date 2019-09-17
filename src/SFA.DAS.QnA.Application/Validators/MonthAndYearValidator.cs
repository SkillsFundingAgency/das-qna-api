using System;
using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class MonthAndYearValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errorMessages = new List<KeyValuePair<string, string>>();

            var dateParts = answer.Value.Split(new[]{","}, StringSplitOptions.RemoveEmptyEntries);
            if (dateParts.Length != 2)
            {
                errorMessages.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
                return errorMessages;
            }
            
            var month = dateParts[0];
            var year = dateParts[1];

            if (string.IsNullOrWhiteSpace(month) || string.IsNullOrWhiteSpace(year) || year.Length != 4)
            {
                errorMessages.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
                return errorMessages;
            }

            if (int.Parse(month) < 1 || int.Parse(month) > 12)
            {
                errorMessages.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
            }

            return errorMessages;
        }
    }
}