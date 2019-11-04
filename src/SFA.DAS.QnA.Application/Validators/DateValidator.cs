using System;
using System.Collections.Generic;
using System.Globalization;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class DateValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            var text = answer?.Value?.Trim();
            var dateParts = text?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            if (string.IsNullOrEmpty(text) || dateParts is null || dateParts.Length != 3)
            {
                errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
            }
            else
            {
                var day = dateParts[0];
                var month = dateParts[1];
                var year = dateParts[2];

                if (string.IsNullOrWhiteSpace(day) || string.IsNullOrWhiteSpace(month) || string.IsNullOrWhiteSpace(year))
                {
                    errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
                }
                else
                {
                    var dateString = $"{day}/{month}/{year}";
                    var formatStrings = new string[] { "d/M/yyyy" };

                    if (!DateTime.TryParseExact(dateString, formatStrings, null, DateTimeStyles.None, out _))
                    {
                        errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
                    }
                }
            }
            
            return errors;
        }
    }
}