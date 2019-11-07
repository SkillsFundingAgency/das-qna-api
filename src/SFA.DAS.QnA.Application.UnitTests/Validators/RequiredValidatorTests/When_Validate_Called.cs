using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Validators;

namespace SFA.DAS.QnA.Application.UnitTests.Validators.RequiredValidatorTests
{
    [TestFixture]
    public class When_Validate_Called
    {
        [TestCase("", false)]
        [TestCase("     ", false)]
        [TestCase(".", true)]
        [TestCase("some input", true)]
        [TestCase("    some input", true)]
        public void Then_correct_errors_are_returned(string input, bool isValid)
        {
            var validator = new RequiredValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "Input is required",
                    Name = "Required"
                }
            };

            var question = new Question { QuestionId = "Q1" };
            var errors = validator.Validate(question, new Answer { Value = input, QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }
    }
}
