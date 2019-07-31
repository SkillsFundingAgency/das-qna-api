using System;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application;
using SFA.DAS.QnA.Application.Commands.StartApplication;

namespace SFA.DAS.QnA.Api.UnitTests.ApplicationControllerTests.StartApplication
{
    [TestFixture]
    public class When_StartApplication_is_called_for_nonexistant_workflowtype
    {
        protected ApplicationController Controller;
        protected IMediator Mediator;
        protected Guid ApplicationId;

        [SetUp]
        public void SetUp()
        {
            Mediator = Substitute.For<IMediator>();
            
            Controller = new ApplicationController(Mediator);

            ApplicationId = Guid.NewGuid();
            Mediator.Send(Arg.Any<StartApplicationRequest>()).Returns(new HandlerResponse<StartApplicationResponse>() {Success = false});
        }

        [Test]
        public void Then_BadRequest_is_returned()
        {
            var result = Controller.StartApplication(new StartApplicationRequest(){UserReference = "dave", WorkflowType = "EPAO"});
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
    
    [TestFixture]
    public class When_StartApplication_is_called
    {
        protected ApplicationController Controller;
        protected IMediator Mediator;
        protected Guid ApplicationId;

        [SetUp]
        public void SetUp()
        {
            Mediator = Substitute.For<IMediator>();
            
            Controller = new ApplicationController(Mediator);

            ApplicationId = Guid.NewGuid();
            Mediator.Send(Arg.Any<StartApplicationRequest>()).Returns(new HandlerResponse<StartApplicationResponse>() {Value = new StartApplicationResponse() {ApplicationId = ApplicationId}});
        }

        [Test]
        public void Then_CreatedAtActionResult_is_returned()
        {
            var result = Controller.StartApplication(new StartApplicationRequest(){UserReference = "dave", WorkflowType = "EPAO"});
            result.Result.Should().BeOfType<CreatedAtActionResult>();
        }
    }
}