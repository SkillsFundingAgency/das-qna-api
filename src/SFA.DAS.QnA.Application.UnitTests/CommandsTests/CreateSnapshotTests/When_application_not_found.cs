using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands.CreateSnapshot;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.CreateSnapshotTests
{
    public class When_application_not_found : CreateSnapshotTestBase
    {
        [Test]
        [Ignore("Must be ran on local DEV machine")]
        public async Task Then_snapshot_fails_with_reason_why()
        {
            var snapshot = await Handler.Handle(new CreateSnapshotRequest(Guid.NewGuid()), new System.Threading.CancellationToken());

            Assert.IsFalse(snapshot.Success);
            Assert.IsNotNull(snapshot.Message);
        }
    }
}
