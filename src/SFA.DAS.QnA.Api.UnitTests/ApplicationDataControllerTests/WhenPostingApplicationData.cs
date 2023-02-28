using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.SetApplicationData;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Api.UnitTests.ApplicationDataControllerTests
{
    public class WhenPostingApplicationData
    {
        [Test]
        public async Task And_MediatorCallFails_ThenNotFoundResultReturned()
        {
            var mediator = Substitute.For<IMediator>();
            var handlerResponseDeserializer = new HandlerResponseDeserializer();
            var controller = new ApplicationDataController(mediator, handlerResponseDeserializer);
            var applicationData = File.ReadAllText("ApplicationDataControllerTests/test.json");
            mediator.Send(Arg.Any<SetApplicationDataRequest>()).Returns(new HandlerResponse<string>() { Success = false, Message = "ApplicationData does not validated against the Project's Schema." });

            var result = await controller.Post(Guid.NewGuid(), applicationData);

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task And_MediatorCallIsSuccessful_ThenDeserializedResultReturned()
        {
            var mediator = Substitute.For<IMediator>();
            var handlerResponseDeserializer = new HandlerResponseDeserializer();
            var controller = new ApplicationDataController(mediator, handlerResponseDeserializer);
            var applicationData = File.ReadAllText("ApplicationDataControllerTests/test.json");
            var handlerResponseValue = new HandlerResponse<string>() { Value = applicationData, Message = "Test Message", Success = true };
            mediator.Send(Arg.Any<SetApplicationDataRequest>()).Returns(handlerResponseValue);

            var actionResult = await controller.Post(Guid.NewGuid(), handlerResponseValue);

            actionResult.Should().BeOfType<ActionResult<object>>();
        }
    }
}