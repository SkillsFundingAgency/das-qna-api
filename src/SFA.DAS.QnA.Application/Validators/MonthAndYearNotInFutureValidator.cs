using System;
using System.Collections.Generic;
using System.Globalization;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class MonthAndYearNotInFutureValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errorMessages = new List<KeyValuePair<string, string>>();

            var dateParts = answer.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (dateParts.Length != 2)
            {
                return errorMessages;
            }

            var day = "1";
            var month = dateParts[0];
            var year = dateParts[1];

            if (string.IsNullOrWhiteSpace(day) || string.IsNullOrWhiteSpace(month) || string.IsNullOrWhiteSpace(year))
            {
                return errorMessages;
            }

            var formatStrings = new string[] { "d/M/yyyy" };
            if (DateTime.TryParseExact($"{day}/{month}/{year}", formatStrings, null, DateTimeStyles.None, out DateTime dateEntered))
            {
                if (dateEntered > DateTime.Today)
                {
                    errorMessages.Add(new KeyValuePair<string, string>(question.QuestionId,
                        ValidationDefinition.ErrorMessage));
                }
            }

            return errorMessages;
        }
    }
}