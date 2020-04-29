namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SubmitPageOfFilesHandlerTests
{
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http.Internal;
    using NUnit.Framework;
    using SFA.DAS.QnA.Application.Commands.Files.UploadFile;

    public class When_page_not_found : SubmitPageOfFilesTestBase
    {
        [Test]
        public async Task Then_validation_error_occurs()
        {
            var response = await Handler.Handle(new SubmitPageOfFilesRequest(ApplicationId, SectionId, "NOT_FOUND", new FormFileCollection
            {
                GenerateFile("This is a dummy file", "Q1", "File.txt")
            }), CancellationToken.None);

            response.Success.Should().BeFalse();
        }
    }
}
