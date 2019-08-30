using System;
using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class FileTypeValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var allowedExtension = ValidationDefinition.Value.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries)[0];

            var fileNameParts = answer.Value.Split(".", StringSplitOptions.RemoveEmptyEntries);
            var fileNameExtension = fileNameParts[fileNameParts.Length - 1];
                    
            if (fileNameExtension != allowedExtension)
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