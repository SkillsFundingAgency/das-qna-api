using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.SetApplicationData;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Api.UnitTests.ApplicationDataControllerTests
{
    public class WhenPostingApplicationData
    {
        [Test]
        public async Task And_MediatorCallFails_ThenNotFoundResultReturned()
        {
            var mediator = Substitute.For<IMediator>();
            var controller = new ApplicationDataController(mediator);
            var applicationData = File.ReadAllText("ApplicationDataControllerTests/test.json");
            mediator.Send(Arg.Any<SetApplicationDataRequest>()).Returns(new HandlerResponse<string>() { Success = false, Message = "ApplicationData does not validated against the Project's Schema." });

            var result = await controller.Post(Guid.NewGuid(), applicationData);

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task And_MediatorCallIsSuccessful_ThenDeserializedResultReturned()
        {
            var mediator = Substitute.For<IMediator>();
            var controller = new ApplicationDataController(mediator);
            var applicationData = File.ReadAllText("ApplicationDataControllerTests/test.json");
            var applicationDataDeserialized = JsonConvert.DeserializeObject(applicationData);
            mediator.Send(Arg.Any<SetApplicationDataRequest>()).Returns(new HandlerResponse<string>(applicationData));

            var actionResult = await controller.Post(Guid.NewGuid(), applicationData);
            var resultAsJson = JsonDocument.Parse(actionResult.Value.ToString()).RootElement;

            actionResult.Value.Should().BeEquivalentTo(applicationDataDeserialized);
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