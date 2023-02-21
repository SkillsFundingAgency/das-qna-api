using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SFA.DAS.QnA.Configuration.Infrastructure;

namespace SFA.DAS.QnA.Configuration.UnitTests.Infrastructure
{
    public class WhenLoadingAzureConfiguration
    {
        [Test]
        public void ThenTheDataIsAddedToCorrectly()
        {
            var configItem = "{\r\n  \"QnA\": {\r\n \"SqlConnectionstring\": \"Data Source=appleDatabase;Initial Catalog=pearDatabase;Integrated Security=True\"\r\n  },\r\n  \"AzureActiveDirectoryConfiguration\": {\r\n    \"Tenant\": \"potatot.test.com\",\r\n    \"Identifier\": \"https://test.config.com/,https://test.fruit.com/\"\r\n  },\r\n  \"NotificationsApiClientConfiguration\": {\r\n    \"ApiBaseUrl\": \"https://test.config/\",\r\n    \"ClientToken\": \"apple.pear.potato-test\",\r\n },\r\n  \"FileStorage\": {\r\n    \"FileEncryptionKey\": \"banana-potato-french\",\r\n }\r\n}";
            var parseConfig = JObject.Parse(configItem);
            var data = new Dictionary<string, string>();
            var expected = BuildExpectedDictionary();

            var result = ConfigHelper.AddKeyValuePairsToDictionary(parseConfig, data);

            Assert.That(result, Is.EquivalentTo(expected));
        }

        private IDictionary<string, string> BuildExpectedDictionary()
        {
            return new Dictionary<string, string>()
            {
                { "QnA:SqlConnectionstring", "Data Source=appleDatabase;Initial Catalog=pearDatabase;Integrated Security=True"},
                { "AzureActiveDirectoryConfiguration:Tenant", "potatot.test.com"},
                { "AzureActiveDirectoryConfiguration:Identifier", "https://test.config.com/,https://test.fruit.com/"},
                { "NotificationsApiClientConfiguration:ApiBaseUrl", "https://test.config/"},
                { "NotificationsApiClientConfiguration:ClientToken", "apple.pear.potato-test"},
                { "FileStorage:FileEncryptionKey", "banana-potato-french"},
            };
        }
    }
}
