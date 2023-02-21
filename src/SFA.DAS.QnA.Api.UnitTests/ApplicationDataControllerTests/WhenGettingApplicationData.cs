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

namespace SFA.DAS.QnA.Api.UnitTests.ApplicationDataControllerTests
{
    [TestFixture]
    public class WhenGettingApplicationData
    {
        protected ApplicationDataController controller;
        protected IMediator mediator;
        protected Guid applicationId;

        [SetUp]
        public void SetUp()
        {
            mediator = Substitute.For<IMediator>();
            controller = new ApplicationDataController(mediator);
            applicationId = Guid.NewGuid();
        }

        [Test]
        public void And_MediatorCallFails_ThenNotFoundResultReturned()
        {
            mediator.Send(Arg.Any<GetApplicationDataRequest>()).Returns(new HandlerResponse<string>() { Success = false, Message = "Application does not exist." });

            var result = controller.Get(applicationId);

            result.Result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public void And_MediatorCallIsSuccessful_ThenOkObjectResultReturned()
        {
            var applicationData = "{\r\n  \"QnA\": {\r\n \"SqlConnectionstring\": \"Data Source=appleDatabase;Initial Catalog=pearDatabase;Integrated Security=True\"\r\n  },\r\n  \"AzureActiveDirectoryConfiguration\": {\r\n    \"Tenant\": \"potatot.test.com\",\r\n    \"Identifier\": \"https://test.config.com/,https://test.fruit.com/\"\r\n  },\r\n  \"NotificationsApiClientConfiguration\": {\r\n    \"ApiBaseUrl\": \"https://test.config/\",\r\n    \"ClientToken\": \"apple.pear.potato-test\"\r\n },\r\n  \"FileStorage\": {\r\n    \"FileEncryptionKey\": \"banana-potato-french\"\r\n }\r\n}";
            var deserializedApplicationData = JsonConvert.DeserializeObject(applicationData).ToString();

            mediator.Send(Arg.Any<GetApplicationDataRequest>()).Returns(new HandlerResponse<string>(applicationData));

            var result = controller.Get(applicationId);

            result.Result.Result.Should().BeOfType<OkObjectResult>();
        }

    }
}
