using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Internal;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands.Files.UploadFile;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SubmitPageOfFilesHandlerTests
{
    public class When_files_specified_is_empty : SubmitPageOfFilesTestBase
    {
        [Test]
        public async Task Then_validation_passes()
        {
            var files = new FormFileCollection();
            var response = await Handler.Handle(new SubmitPageOfFilesRequest(ApplicationId, SectionId, "1", files), CancellationToken.None);

            response.Value.ValidationPassed.Should().BeTrue();
        }
    }
}
