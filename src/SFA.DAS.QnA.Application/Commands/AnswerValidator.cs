using System.Collections.Generic;
using System.Linq;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Application.Validators;

namespace SFA.DAS.QnA.Application.Commands
{
    public class AnswerValidator : IAnswerValidator
    {
        private readonly IValidatorFactory _validatorFactory;

        public AnswerValidator(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        public List<KeyValuePair<string, string>> Validate(List<Answer> answers, Page page)
        {
            var validationErrors = new List<KeyValuePair<string, string>>();
            foreach (var question in page.Questions)
            {
                var answerToThisQuestion = answers.SingleOrDefault(a => a.QuestionId == question.QuestionId);

                ValidateQuestion(question, validationErrors, answerToThisQuestion);

                if (question.Input.Options == null) continue;

                foreach (var option in question.Input.Options.Where(option => answerToThisQuestion?.Value == option.Value && option.FurtherQuestions != null))
                {
                    foreach (var furtherQuestion in option.FurtherQuestions)
                    {
                        var furtherAnswer = answers.FirstOrDefault(a => a.QuestionId == furtherQuestion.QuestionId);
                        ValidateQuestion(furtherQuestion, validationErrors, furtherAnswer);
                    }
                }
            }

            return validationErrors;
        }

        private void ValidateQuestion(Question question, List<KeyValuePair<string, string>> validationErrors, Answer answerToThisQuestion)
        {
            var validators = _validatorFactory.Build(question);
            
            if (answerToThisQuestion is null || answerToThisQuestion.Value == "")
            {
                if (validators.Any(v => typeof(RequiredValidator).Name.Equals(v.GetType().Name)))
                {
                    var validator = validators.First(v => typeof(RequiredValidator).Name.Equals(v.GetType().Name));
                    var errors = validator.Validate(question, answerToThisQuestion);
                    if (errors.Any())
                    {
                        validationErrors.AddRange(errors);
                    }
                }
            }
            else
            {
                foreach (var validator in validators)
                {
                    var errors = validator.Validate(question, answerToThisQuestion);

                    if (errors.Any())
                    {
                        validationErrors.AddRange(errors);
                    }
                }
            }
        }
    }
}