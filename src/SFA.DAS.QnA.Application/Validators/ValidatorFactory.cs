using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class ValidatorFactory : IValidatorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidatorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public List<IValidator> Build(Question question)
        {
            var validators = new List<IValidator>();

            if (question?.Input != null)
            {
                var hasInputValidatorsSpecified = question.Input.Validations != null && question.Input.Validations.Count > 0;

                var typeValidator = _serviceProvider.GetServices<IValidator>().FirstOrDefault(v => v.GetType().Name == $"{question.Input.Type}TypeValidator");

                if (typeValidator != null)
                {
                    var isTypeValidatorOverridden = hasInputValidatorsSpecified && question.Input.Validations.Any(v => v.Name == $"{question.Input.Type}Validator");

                    if(!isTypeValidatorOverridden)
                    {
                        validators.Add(typeValidator);
                    }
                }

                if (hasInputValidatorsSpecified)
                {
                    foreach (var inputValidation in question.Input.Validations.Where(v => v.Name != "ClientApiCall"))
                    {
                        var validator = _serviceProvider.GetServices<IValidator>().FirstOrDefault(v => v.GetType().Name == $"{inputValidation.Name}Validator");

                        if (validator != null)
                        {
                            validator.ValidationDefinition = inputValidation;
                            validators.Add(validator);
                        }
                    }
                }

                if (validators.Count == 0)
                {
                    validators.Add(new NullValidator());
                }
            }

            return validators;
        }
    }
}