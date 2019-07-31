using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application;
using SFA.DAS.QnA.Application.Queries.Sequences.GetCurrentSequence;

namespace SFA.DAS.QnA.Api.UnitTests.SequencesControllerTests.GetCurrentSequence
{
    [TestFixture]
    public class When_GetCurrentSequence_is_called_for_application_with_no_current_sequence : GetCurrentSequenceTestBase
    {
        [Test]
        public async Task Then_NoContent_is_returned()
        {
            Mediator.Send(Arg.Any<GetCurrentSequenceRequest>(), Arg.Any<CancellationToken>())
                .Returns(new HandlerResponse<Sequence>
                {
                    Value = null
                });

            var result = await Controller.GetCurrentSequence(Guid.NewGuid());

            result.Result.Should().BeOfType<NoContentResult>();
        }
    }
}