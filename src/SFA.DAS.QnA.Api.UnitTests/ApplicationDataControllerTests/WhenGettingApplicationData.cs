using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.SystemFunctions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Api.UnitTests.ApplicationDataControllerTests
{
    [TestFixture]
    public class WhenGettingApplicationData
    {
        [Test]
        public async Task And_MediatorCallFails_ThenNotFoundResultReturned()
        {
            var mediator = Substitute.For<IMediator>();
            var sut = new ApplicationDataController(mediator);

            mediator.Send(Arg.Any<GetApplicationDataRequest>()).Returns(new HandlerResponse<string>() { Success = false, Message = "Application does not exist." });

            var result = await sut.Get(Guid.NewGuid());

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task And_MediatorCallIsSuccessful_ThenOkObjectResultReturned()
        {
            var mediator = Substitute.For<IMediator>();
            var sut = new ApplicationDataController(mediator);
            var applicationData = File.ReadAllText("ApplicationDataControllerTests/test.json");
            var applicationDataDeserialized = JsonConvert.DeserializeObject(applicationData);
            mediator.Send(Arg.Any<GetApplicationDataRequest>()).Returns(new HandlerResponse<string>(applicationData));

            var result = await sut.Get(Guid.NewGuid());
            ObjectResult objectResult = (ObjectResult)result.Result;

            result.Result.Should().BeOfType<OkObjectResult>();
            objectResult.Value.Should().BeEquivalentTo(applicationDataDeserialized);
        }

        // Feel free to modify this to use your preferred paradigms like FluentAssertions,
        // but it's easier to do multiple closely-related assertions in plain NUnit.
        [Test]
        public async Task And_MediatorCallIsSuccessful_Then_Result_Contains_ExpectedProperties()
        {
            var applicationData = File.ReadAllText("ApplicationDataControllerTests/test.json");
            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<GetApplicationDataRequest>()).Returns(new HandlerResponse<string>(applicationData));

            var sut = new ApplicationDataController(mediator);
            var actionResult = await sut.Get(Guid.NewGuid());
            var okResult = (OkObjectResult)actionResult.Result; // hard cast to OkObjectResult, since this test isn't checking for type correctness

            // Convert the result to a JsonDocument.
            // Here I'm using Text.Json, which shouldn't matter since I'm using it as a tool to access properties
            // The reason I asked you to use Newtonsoft Json is because I thought it might be easier to test Newtonsoft code with Newtonsoft.
            // Turns out that's not necessary for this test.
            var resultAsJson = JsonDocument.Parse(okResult.Value.ToString()).RootElement;

            // Test a few properties. Here I've tested one of each kind.
            Assert.Multiple(() =>
            {
                // Test a string property. ID looks important.
                Assert.That(resultAsJson.GetProperty("OrganisationReferenceId")
                                        .GetString(),
                                        Is.EqualTo("c3333b62-a07c-415e-8778-84222231b0s1"));

                // Test a null
                Assert.That(resultAsJson.GetProperty("TradingName")
                                        .ValueKind,
                                        Is.EqualTo(JsonValueKind.Null));

                // Test a boolean
                Assert.That(resultAsJson.GetProperty("UseTradingName")
                                        .GetBoolean(),
                                        Is.False);

                // Test a blank value
                Assert.That(resultAsJson.GetProperty("company_number")
                                        .GetString(),
                                        Is.Empty);

                // Test a sub-property
                Assert.That(resultAsJson.GetProperty("CompanySummary")
                                        .GetProperty("CompanyNumber")
                                        .GetString(),
                                        Is.EqualTo("RC123456"));

                // Test an array
                Assert.That(resultAsJson.GetProperty("CharitySummary")
                                        .GetProperty("Trustees") // the array node is inside this sub-property
                                        .EnumerateArray() // interpret the node as an array so that we can access it like any old collection
                                        .First()
                                        .GetProperty("Name")
                                        .GetString(),
                                        Is.EqualTo("DR DOOM"));
            });
        }
    }
}
