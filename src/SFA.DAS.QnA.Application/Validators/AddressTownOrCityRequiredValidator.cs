using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class AddressTownOrCityRequiredValidator : AddressRequiredValidatorBase, IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            return ValidateProperty(question.QuestionId, answer.Value, "AddressLine3", ValidationDefinition.ErrorMessage);
        }
    }
}