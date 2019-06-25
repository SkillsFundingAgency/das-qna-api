using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.Qna.Api.Types;
using SFA.DAS.QnA.Application;
using SFA.DAS.QnA.Application.Queries.Sequences.GetCurrentSequence;

namespace SFA.DAS.QnA.Api.UnitTests.SequencesControllerTests.GetCurrentSequence
{
    public class When_GetCurrentSequence_is_called_for_nonexistant_application : GetCurrentSequenceTestBase
    {
        [Test]
        public async Task Then_NotFound_is_returned()
        {
            Mediator.Send(Arg.Any<GetCurrentSequenceRequest>(), Arg.Any<CancellationToken>())
                .Returns(new HandlerResponse<Sequence>
                {
                    Success = false,
                    Message = "Application does not exist"
                });
            
            var result = await Controller.GetCurrentSequence(Guid.NewGuid());

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}