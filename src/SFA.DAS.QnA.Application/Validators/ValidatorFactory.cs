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
                var typeValidator = _serviceProvider.GetServices<IValidator>()
                    .FirstOrDefault(v => v.GetType().Name == $"{question.Input.Type}TypeValidator");

                if (typeValidator != null)
                {
                    if (!string.IsNullOrEmpty(question.Input.ErrorMessage))
                    {
                        // Override the default ErrorMessage if one was specified
                        typeValidator.ValidationDefinition.ErrorMessage = question.Input.ErrorMessage;
                    }

                    validators.Add(typeValidator);
                }

                if (question.Input.Validations != null && question.Input.Validations.Any())
                {
                    foreach (var inputValidation in question.Input.Validations.Where(v => v.Name != "ClientApiCall"))
                    {
                        var validator = _serviceProvider.GetServices<IValidator>()
                            .FirstOrDefault(v => v.GetType().Name == $"{inputValidation.Name}Validator");

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