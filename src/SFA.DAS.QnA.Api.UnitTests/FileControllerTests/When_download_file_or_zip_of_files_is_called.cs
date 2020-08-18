using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.Files.DownloadFile;

namespace SFA.DAS.QnA.Api.UnitTests.FileControllerTests
{
    [TestFixture]
    public class When_download_file_or_zip_of_files_is_called
    {
        private IMediator _mediator;
        private IHttpContextAccessor _httpContextAccessor;
        private FileController _fileController;

        [SetUp]
        public void SetUp()
        {
            _mediator = Substitute.For<IMediator>();
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            _fileController = new FileController(_mediator, _httpContextAccessor);
        }

        [Test]
        public async Task Then_the_file_is_returned()
        {
            var expectedApplicationId = Guid.NewGuid();
            var expectedSequenceNo = 123;
            var expectedSectionNo = 456;
            var expectedPageId = "pageId";
            var expectedQuestionId = "questionId";

            var expectedResponse = new DownloadFile
            {
                ContentType = "application/pdf",
                FileName = "fileName",
                Stream = new MemoryStream()
            };

            _mediator.Send(Arg.Is<DownloadFileBySectionNoRequest>(x =>
                    x.ApplicationId == expectedApplicationId && x.SectionNo == expectedSectionNo &&
                    x.SequenceNo == expectedSequenceNo && x.PageId == expectedPageId &&
                    x.QuestionId == expectedQuestionId))
                .Returns(new HandlerResponse<DownloadFile>(expectedResponse));

            var result = await _fileController.DownloadFileOrZipOfFiles(expectedApplicationId, expectedSequenceNo, expectedSectionNo, expectedPageId, expectedQuestionId) as FileStreamResult;

            result.FileStream.Should().Be(expectedResponse.Stream);
            result.ContentType.Should().Be(expectedResponse.ContentType);
            result.FileDownloadName.Should().Be(expectedResponse.FileName);
        }
    }
}
