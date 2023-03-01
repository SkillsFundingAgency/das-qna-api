using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace SFA.DAS.QnA.Api.Types.UnitTests
{
    public class WhenDeserializingModelValue
    {
        [TestMethod]
        public void ThenValueIsDeserializedCorrectly()
        {
            var modelValue = GetNonNullModelValue(); // generates a sample Model.Value that's reasonably realistic

            var tableData = new SFA.DAS.QnA.Api.Types.Page.ModelValueDeserializer();

            Assert.That(tableData.HeadingTitles, Is.Not.Null);
            Assert.That(tableData.DataRows, Is.Not.Null);
        }

        private object GetNonNullModelValue()
        {
            throw new NotImplementedException();
        }
    }
}