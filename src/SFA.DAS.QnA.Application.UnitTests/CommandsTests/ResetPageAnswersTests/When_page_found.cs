namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.ResetPageAnswersTests
{
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using SFA.DAS.QnA.Application.Commands.ResetPageAnswers;
    using SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData;
    using SFA.DAS.QnA.Application.Queries.Sections.GetPage;

    public class When_page_found : ResetPageAnswersTestBase
    {
        [Test]
        public async Task Then_successful_response()
        {
            var response = await Handler.Handle(new ResetPageAnswersRequest(ApplicationId, SectionId, "1"), CancellationToken.None);

            response.Value.HasPageAnswersBeenReset.Should().BeTrue();
        }

        [Test]
        public async Task Then_page_answers_are_reset()
        {
            await Handler.Handle(new ResetPageAnswersRequest(ApplicationId, SectionId, "1"), CancellationToken.None);

            var getPageResponse = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, "1"), CancellationToken.None);
            getPageResponse.Value.PageOfAnswers.Should().BeEmpty();
        }

        [Test]
        public async Task Then_page_complete_is_false()
        {
            await Handler.Handle(new ResetPageAnswersRequest(ApplicationId, SectionId, "1"), CancellationToken.None);

            var getPageResponse = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, "1"), CancellationToken.None);
            getPageResponse.Value.Complete.Should().BeFalse();
        }

        [Test]
        public async Task Then_questiontag_is_reset()
        {
            await Handler.Handle(new ResetPageAnswersRequest(ApplicationId, SectionId, "1"), CancellationToken.None);

            var getApplicationDataResponse = await GetApplicationDataHandler.Handle(new GetApplicationDataRequest(ApplicationId), CancellationToken.None);

            var applicationData = JObject.Parse(getApplicationDataResponse.Value);
            var questionTag = applicationData["Q1"];

            questionTag.Should().NotBeNull();
            questionTag.Value<string>().Should().BeNullOrEmpty();
        }
    }
}
