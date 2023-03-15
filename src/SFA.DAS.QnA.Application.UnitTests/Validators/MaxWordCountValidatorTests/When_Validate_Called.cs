using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Validators;

namespace SFA.DAS.QnA.Application.UnitTests.Validators.MaxWordCountValidatorTests
{
    [TestFixture]
    public class When_Validate_Called
    {
        [TestCase("", "10", true)]
        [TestCase("Mary had a little lamb", "10", true)]
        [TestCase("    Mary  had   a   little lamb ", "10", true)]
        [TestCase("Mary had a little lamb, its fleece was white as snow", "10", false)]
        [TestCase("   Mary had a     little lamb, its fleece was white as snow                   ", "10", false)]
        [TestCase("\r\none two three four \r\nfive-six-seven eight nine ten\r\n", "10", true)]
        [TestCase(" \r\n one two three four \r\nfive-six-seven eight nine ten \r\n ", "10", true)]
        public void Then_correct_errors_are_returned(string input, string wordLimit, bool isValid)
        {
            var validator = new MaxWordCountValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "Word count exceeded",
                    Name = "MaxWordCount",
                    Value = wordLimit
                }
            };

            var question = new Question { QuestionId = "Q1" };
            var errors = validator.Validate(question, new Answer { Value = input, QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }
    }
}
