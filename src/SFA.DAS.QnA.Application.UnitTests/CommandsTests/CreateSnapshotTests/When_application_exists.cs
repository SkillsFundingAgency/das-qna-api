using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands.CreateSnapshot;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.CreateSnapshotTests
{
    public class When_application_exists : CreateSnapshotTestBase
    {
        [Test]
#if (!DEBUG)
        [Ignore("Must be tested on local DEV machine as it uses local Azure storage")]
#endif
        public async Task Then_snapshot_is_created()
        {
            var snapshot = await Handler.Handle(new CreateSnapshotRequest(ApplicationId), new System.Threading.CancellationToken());

            snapshot.Success.Should().BeTrue();
            snapshot.Value.ApplicationId.Should().NotBe(ApplicationId);
        }

        [Test]
#if (!DEBUG)
        [Ignore("Must be tested on local DEV machine as it uses local Azure storage")]
#endif
        public async Task Then_snapshot_has_copied_over_files_successfully()
        {
            var snapshot = await Handler.Handle(new CreateSnapshotRequest(ApplicationId), new System.Threading.CancellationToken());
            snapshot.Success.Should().BeTrue();

            var section = DataContext.ApplicationSections.SingleOrDefault(sec => sec.ApplicationId == snapshot.Value.ApplicationId);
            var page = section?.QnAData.Pages.SingleOrDefault(p => p.PageId == PageId);
            var answer = page?.PageOfAnswers.SelectMany(pao => pao.Answers).SingleOrDefault(ans => ans.QuestionId == QuestionId);

            answer.Should().NotBeNull();
            answer.Value.Should().Be(Filename);
            (await FileExists(section.ApplicationId, section.SequenceId, section.Id, page.PageId, answer.QuestionId, answer.Value, ContainerClient)).Should().BeTrue();
        }
    }
}
