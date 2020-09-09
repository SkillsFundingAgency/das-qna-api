using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands.ResetPageAnswers;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.ResetPageAnswersHandlerTests
{
    public class When_page_not_found : ResetPageAnswersHandlerTests
    {
        [Test]
        public async Task Then_validation_error_occurs()
        {
            var response = await Handler.Handle(new ResetPageAnswersRequest(ApplicationId, SectionId, "NOT_FOUND"), CancellationToken.None);

            response.Success.Should().BeFalse();
        }
    }
}
