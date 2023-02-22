using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Api.UnitTests.ApplicationDataControllerTests
{
    [TestFixture]
    public class WhenGettingApplicationData
    {
        protected ApplicationDataController sut;
        protected IMediator mediator;
        protected Guid applicationId;

        [Test]
        public async Task And_MediatorCallFails_ThenNotFoundResultReturned()
        {
            mediator = Substitute.For<IMediator>();
            sut = new ApplicationDataController(mediator);

            mediator.Send(Arg.Any<GetApplicationDataRequest>()).Returns(new HandlerResponse<string>() { Success = false, Message = "Application does not exist." });

            var result = await sut.Get(Guid.NewGuid());

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task And_MediatorCallIsSuccessful_ThenOkObjectResultReturned()
        {
            mediator = Substitute.For<IMediator>();
            sut = new ApplicationDataController(mediator);

            var applicationData = "{\r\n  \"QnA\": {\r\n \"SqlConnectionstring\": \"Data Source=appleDatabase;Initial Catalog=pearDatabase;Integrated Security=True\"\r\n  },\r\n  \"AzureActiveDirectoryConfiguration\": {\r\n    \"Tenant\": \"potatot.test.com\",\r\n    \"Identifier\": \"https://test.config.com/,https://test.fruit.com/\"\r\n  },\r\n  \"NotificationsApiClientConfiguration\": {\r\n    \"ApiBaseUrl\": \"https://test.config/\",\r\n    \"ClientToken\": \"apple.pear.potato-test\"\r\n },\r\n  \"FileStorage\": {\r\n    \"FileEncryptionKey\": \"banana-potato-french\"\r\n }\r\n}";

            mediator.Send(Arg.Any<GetApplicationDataRequest>()).Returns(new HandlerResponse<string>(applicationData));

            var result = await sut.Get(applicationId);

            result.Result.Should().BeOfType<OkObjectResult>();
        }

    }
}
