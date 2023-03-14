using System;
using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class MaxWordCountValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            var text = answer?.Value?.Trim();

            if (!string.IsNullOrEmpty(text))
            {
                var splitters = new string[] { " ", "\r", "\n" };

                var wordCount = text.Split(splitters, StringSplitOptions.RemoveEmptyEntries).Length;

                if (wordCount > long.Parse(ValidationDefinition.Value))
                {
                    errors.Add(new KeyValuePair<string, string>(answer.QuestionId, ValidationDefinition.ErrorMessage));
                }
            }

            return errors;
        }
    }
}
