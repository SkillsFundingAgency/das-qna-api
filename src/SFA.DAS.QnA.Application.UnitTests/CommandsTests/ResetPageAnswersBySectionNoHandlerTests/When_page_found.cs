﻿using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands.ResetPageAnswers;
using SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData;
using SFA.DAS.QnA.Application.Queries.Sections.GetPage;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.ResetPageAnswersBySectionNoHandlerTests
{
    public class When_page_found : ResetPageAnswersBySectionNoTestBase
    {
        [Test]
        public async Task Then_successful_response()
        {
            var response = await Handler.Handle(new ResetPageAnswersBySectionNoRequest(ApplicationId, SequenceNo, SectionNo, "1"), CancellationToken.None);

            response.Value.HasPageAnswersBeenReset.Should().BeTrue();
        }

        [Test]
        public async Task Then_page_answers_are_reset()
        {
            await Handler.Handle(new ResetPageAnswersBySectionNoRequest(ApplicationId, SequenceNo, SectionNo, "1"), CancellationToken.None);

            var getPageResponse = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, "1"), CancellationToken.None);
            getPageResponse.Value.PageOfAnswers.Should().BeEmpty();
        }

        [Test]
        public async Task Then_page_complete_is_false()
        {
            await Handler.Handle(new ResetPageAnswersBySectionNoRequest(ApplicationId, SequenceNo, SectionNo, "1"), CancellationToken.None);

            var getPageResponse = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, "1"), CancellationToken.None);
            getPageResponse.Value.Complete.Should().BeFalse();
        }

        [Test]
        public async Task Then_questiontag_is_reset()
        {
            await Handler.Handle(new ResetPageAnswersBySectionNoRequest(ApplicationId, SequenceNo, SectionNo, "1"), CancellationToken.None);

            var getApplicationDataResponse = await GetApplicationDataHandler.Handle(new GetApplicationDataRequest(ApplicationId), CancellationToken.None);

            var applicationData = JsonDocument.Parse(getApplicationDataResponse.Value).RootElement;
            var questionTag = applicationData.GetProperty("Q1");

            questionTag.ValueKind.Should().Be(JsonValueKind.Null);
        }

        [Test]
        public async Task Then_all_pages_have_their_active_status_set_correctly()
        {
            await Handler.Handle(new ResetPageAnswersBySectionNoRequest(ApplicationId, SequenceNo, SectionNo, "1"), CancellationToken.None);

            var page1Response = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, "1"), CancellationToken.None);
            page1Response.Value.Active.Should().BeTrue();

            var page2Response = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, "2"), CancellationToken.None);
            page2Response.Value.Active.Should().BeFalse();

            var page3Response = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, "3"), CancellationToken.None);
            page3Response.Value.Active.Should().BeFalse();
        }
    }
}
