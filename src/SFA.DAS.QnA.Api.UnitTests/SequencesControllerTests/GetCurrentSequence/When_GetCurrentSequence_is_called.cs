using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.Qna.Api.Types;
using SFA.DAS.QnA.Application;
using SFA.DAS.QnA.Application.Queries.Sequences.GetCurrentSequence;

namespace SFA.DAS.QnA.Api.UnitTests.SequencesControllerTests.GetCurrentSequence
{
    [TestFixture]
    public class When_GetCurrentSequence_is_called : GetCurrentSequenceTestBase
    {
        [Test]
        public async Task Then_a_sequence_is_returned()
        {
            var sequenceId = Guid.NewGuid();
            
            Mediator.Send(Arg.Any<GetCurrentSequenceRequest>(), Arg.Any<CancellationToken>())
                .Returns(new HandlerResponse<Sequence>
                {
                    Value = new Sequence{Id = sequenceId}
                });

            var result = await Controller.GetCurrentSequence(Guid.NewGuid());

            result.Value.Should().BeOfType<Sequence>();
            result.Value.Id.Should().Be(sequenceId);
        }
    }
}