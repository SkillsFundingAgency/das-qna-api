using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SFA.DAS.QnA.Application.Validators
{
    public class AddressRequiredValidatorBase
    {
        protected List<KeyValuePair<string, string>> ValidateProperty(string questionId, string answer, string property, string errorMessage)
        {
            var errorMessages = new List<KeyValuePair<string, string>>();

            try
            {
                var addressObject = JObject.Parse(answer);

                if (addressObject.TryGetValue(property, out var propertyValue))
                {
                    if (string.IsNullOrWhiteSpace(propertyValue.Value<string>()))
                    {
                        errorMessages.Add(new KeyValuePair<string, string>(questionId, errorMessage));
                        return errorMessages;
                    }
                }
                else
                {
                    errorMessages.Add(new KeyValuePair<string, string>(questionId, errorMessage));
                }
            }
            catch (JsonReaderException)
            {
                errorMessages.Add(new KeyValuePair<string, string>(questionId, "Address data is not JSON"));
                
            }

            return errorMessages;
        }
    }
}