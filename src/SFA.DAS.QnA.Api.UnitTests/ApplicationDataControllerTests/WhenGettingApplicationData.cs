using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData;
using System;
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
            var controller = new Controllers.ApplicationDataController(mediator);
            mediator.Send(Arg.Any<GetApplicationDataRequest>()).Returns(new HandlerResponse<string>() { Success = false, Message = "Application does not exist." });

            var result = await controller.Get(Guid.NewGuid());

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task And_MediatorCallIsSuccessful_ThenOkObjectResultReturned()
        {
            var mediator = Substitute.For<IMediator>();
            var controller = new Controllers.ApplicationDataController(mediator);
            var applicationData = File.ReadAllText("ApplicationDataControllerTests/test.json");
            var applicationDataDeserialized = JsonConvert.DeserializeObject(applicationData);
            mediator.Send(Arg.Any<GetApplicationDataRequest>()).Returns(new HandlerResponse<string>(applicationData));

            var result = await controller.Get(Guid.NewGuid());
            ObjectResult objectResult = (ObjectResult)result.Result;

            result.Result.Should().BeOfType<OkObjectResult>();
            objectResult.Value.Should().BeEquivalentTo(applicationDataDeserialized);
        }

        [Test]
        public async Task And_MediatorCallIsSuccessful_Then_Result_Contains_ExpectedProperties()
        {
            var applicationData = File.ReadAllText("ApplicationDataControllerTests/test.json");
            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<GetApplicationDataRequest>()).Returns(new HandlerResponse<string>(applicationData));
            var controller = new Controllers.ApplicationDataController(mediator);

            var actionResult = await controller.Get(Guid.NewGuid());
            var okResult = (OkObjectResult)actionResult.Result;
            var resultAsJson = JsonDocument.Parse(okResult.Value.ToString()).RootElement;

            Assert.Multiple(() =>
            {
                Assert.That(resultAsJson.GetProperty("OrganisationReferenceId")
                                        .GetString(),
                                        Is.EqualTo("c3333b62-a07c-415e-8778-84222231b0s1"));

                Assert.That(resultAsJson.GetProperty("TradingName")
                                        .ValueKind,
                                        Is.EqualTo(JsonValueKind.Null));

                Assert.That(resultAsJson.GetProperty("UseTradingName")
                                        .GetBoolean(),
                                        Is.False);

                Assert.That(resultAsJson.GetProperty("company_number")
                                        .GetString(),
                                        Is.Empty);

                Assert.That(resultAsJson.GetProperty("CompanySummary")
                                        .GetProperty("CompanyNumber")
                                        .GetString(),
                                        Is.EqualTo("RC123456"));

                Assert.That(resultAsJson.GetProperty("CharitySummary")
                                        .GetProperty("Trustees")
                                        .EnumerateArray()
                                        .First()
                                        .GetProperty("Name")
                                        .GetString(),
                                        Is.EqualTo("DR DOOM"));
            });
        }
    }
}