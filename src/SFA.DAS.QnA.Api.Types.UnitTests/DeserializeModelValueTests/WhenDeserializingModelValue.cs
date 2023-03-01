using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Api.Types.UnitTests.DeserializeModelValueTests
{
    public class WhenDeserializingModelValue
    {
        [Test]
        public void ThenDeserializationIsCorrect()
        {
            var value = GetNonNullModelValue();

            var tableData = ModelValueDeserialiser.Deserialize(value);

            Assert.That(tableData.HeadingTitles, Is.Not.Null);
            Assert.That(tableData.DataRows, Is.Not.Null);
        }

        private string GetNonNullModelValue()
        {
            var json = File.ReadAllText("DeserializeModelValueTests/test.json");
            return json;
        }
    }
}