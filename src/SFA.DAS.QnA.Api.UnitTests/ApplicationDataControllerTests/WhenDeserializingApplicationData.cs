using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using SFA.DAS.QnA.Api.Types;
using System.Linq;

namespace SFA.DAS.QnA.Api.UnitTests.ApplicationDataControllerTests
{
    public class WhenDeserializingApplicationData
    {

        [Test]
        public async Task And_MediatorCallIsSuccessful_Then_Result_Contains_ExpectedProperties()
        {
            var handlerResponseDeserializer = new HandlerResponseDeserializer();
            var handlerResponse = new HandlerResponse<string>() { Value = File.ReadAllText("ApplicationDataControllerTests/test.json"), Message = "Test Message", Success = true };

            var response = await handlerResponseDeserializer.Deserialize(handlerResponse);
            var value = JsonDocument.Parse(response.ToString()).RootElement;

            Assert.Multiple(() =>
            {
                Assert.That(value.GetProperty("OrganisationReferenceId")
                                        .GetString(),
                                        Is.EqualTo("c3333b62-a07c-415e-8778-84222231b0s1"));

                Assert.That(value.GetProperty("TradingName")
                                        .ValueKind,
                                        Is.EqualTo(JsonValueKind.Null));

                Assert.That(value.GetProperty("UseTradingName")
                                        .GetBoolean(),
                                        Is.False);

                Assert.That(value.GetProperty("company_number")
                                        .GetString(),
                                        Is.Empty);

                Assert.That(value.GetProperty("CompanySummary")
                                        .GetProperty("CompanyNumber")
                                        .GetString(),
                                        Is.EqualTo("RC123456"));

                Assert.That(value.GetProperty("CharitySummary")
                                        .GetProperty("Trustees")
                                        .EnumerateArray()
                                        .First()
                                        .GetProperty("Name")
                                        .GetString(),
                                        Is.EqualTo("test name 1"));
            });
        }
    }
}
