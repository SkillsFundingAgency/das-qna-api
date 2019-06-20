using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequences;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetSequencesTests
{
    [TestFixture]
    public class When_GetSequences_handled_for_non_existant_application : GetSequencesTestBase
    {
        [Test]
        public async Task Then_no_sequences_are_returned()
        {
            var results = await Handler.Handle(new GetSequencesRequest(Guid.NewGuid()), CancellationToken.None);

            results.Should().BeEmpty();
        }
    }
}