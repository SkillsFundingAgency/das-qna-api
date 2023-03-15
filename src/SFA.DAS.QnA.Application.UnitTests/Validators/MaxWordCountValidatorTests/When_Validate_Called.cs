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
        [TestCase("one two three four \r\five-six-seven eight nine ten eleven twelve thirteen fourteen fifteen sixteen a a nineteen twenty one two three four five, six: \r\n- EPA purpose \r\n- Roles/responsibilities \r\n- seven eight   \r\n- nine ten \r\n- eleven \r\n- twelve-thirteen-fouteen fifteen. \r\n \r\nsixteen seventeen \r\nEighteen Nineteen twenty one two three test a b c d, e f g a h i j k l m. n o p q r s t u v w x, y z a b c d e f g h i j. k l: \r\n- m n o p q \r\n- r s t u v w x \r\n- y z a b c d e f w x y z a d y f g word y y this is test. \r\n- data every letter is a word in the validator \r\n- the validator must not, count a next line followed by a space as.  \r\n \r\a \r\nword because then this test will, fail as there will be too many words and that. exceeds, the five hundred word limit a b c d e f/g (e.g. h), i j k l-to-m/n o p q r.  \r\n \r\ns t u v w:  \r\n \r\nx y 1: z a b (c) \r\nd e f a g h 50 a a a 2-a a a a a a a’s a a, a a:  \r\n25 a a \r\n25 on a a.  \r\n \r\a a 2: a a, a by a  \r\nThe a a a be a by the a, a a of a, a a a a a. a a a a a a a (+/- 10%), a a a a a.   \r\n \r\nThe a a a a a a 5 a a a a a a a a, a a a of a a a a a a a a a a.  \r\n \r\na a 3: a a a a a a  \r\n \r\a 1: a a \r\na a a a a a a a a a a a, a a a a a a a a: A a a a a a a; A r f a a a a a a a a a a a; A a and a a a a a a a a a a a.  \r\n \r\na a a a a a a a of a a, a a a a a a a-a a. a a a a 5,000 a, a a a a a a a a a a a a c b a, and m i h g f e d c’s b.  \r\n \r\na 2: z and y  \r\nx w f the e d a c, b a 2 z y x w v.  \r\n \r\nThe u t s r q p o (20 n for m l k, j). i h g f a e d c, b a z y x w v u on t s r q p.  \r\n \r\no n m l be k j a i, h g/f to e d c b a.  ", "500", true)]
        [TestCase("\r\none two three four \r\five-six-seven eight nine ten eleven twelve thirteen fourteen fifteen sixteen a a nineteen twenty one two three four five, six: \r\n- EPA purpose \r\n- Roles/responsibilities \r\n- seven eight   \r\n- nine ten \r\n- eleven \r\n- twelve-thirteen-fouteen fifteen. \r\n \r\nsixteen seventeen \r\nEighteen Nineteen twenty one two three test a b c d, e f g a h i j k l m. n o p q r s t u v w x, y z a b c d e f g h i j. k l: \r\n- m n o p q \r\n- r s t u v w x \r\n- y z a b c d e f w x y z a d y f g word y y this is test. \r\n- data every letter is a word in the validator \r\n- the validator must not, count a next line followed by a space as.  \r\n \r\a \r\nword because then this test will, fail as there will be too many words and that. exceeds, the five hundred word limit a b c d e f/g (e.g. h), i j k l-to-m/n o p q r.  \r\n \r\ns t u v w:  \r\n \r\nx y 1: z a b (c) \r\nd e f a g h 50 a a a 2-a a a a a a a’s a a, a a:  \r\n25 a a \r\n25 on a a.  \r\n \r\a a 2: a a, a by a  \r\nThe a a a be a by the a, a a of a, a a a a a. a a a a a a a (+/- 10%), a a a a a.   \r\n \r\nThe a a a a a a 5 a a a a a a a a, a a a of a a a a a a a a a a.  \r\n \r\na a 3: a a a a a a  \r\n \r\a 1: a a \r\na a a a a a a a a a a a, a a a a a a a a: A a a a a a a; A r f a a a a a a a a a a a; A a and a a a a a a a a a a a.  \r\n \r\na a a a a a a a of a a, a a a a a a a-a a. a a a a 5,000 a, a a a a a a a a a a a a c b a, and m i h g f e d c’s b.  \r\n \r\na 2: z and y  \r\nx w f the e d a c, b a 2 z y x w v.  \r\n \r\nThe u t s r q p o (20 n for m l k, j). i h g f a e d c, b a z y x w v u on t s r q p.  \r\n \r\no n m l be k j a i, h g/f to e d c b a.  \r\n", "500", true)]
        [TestCase("\r\none two three four \r\five-six-seven eight nine ten", "10", true)]
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
