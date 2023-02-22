using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData;
using System;
using System.IO;
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
    }
}
