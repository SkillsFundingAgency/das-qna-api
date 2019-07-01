using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.Qna.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SetPageAnswersTests
{
    public class When_radio_condition_redirects_to_new_branch : SetPageAnswersTestBase
    {
        [Test]
        public async Task Then_previously_inactive_branch_is_activated()
        {
            await Handler.Handle(new SetPageAnswersRequest(ApplicationId, SectionId, "1", new List<Answer>
            {
                new Answer() {QuestionId = "Q1", Value = "Yes"}
            }), CancellationToken.None);

            var updatedSection = await DataContext.ApplicationSections.SingleAsync();

            updatedSection.QnAData.Pages.Single(page => page.PageId == "1").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "2").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "3").Active.Should().BeFalse();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "4").Active.Should().BeFalse();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "5").Active.Should().BeFalse();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "6").Active.Should().BeFalse();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "7").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "8").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "9").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "10").Active.Should().BeTrue();
        }
        
        [Test]
        public async Task Then_previously_inactive_branch_does_not_get_activated()
        {
            await Handler.Handle(new SetPageAnswersRequest(ApplicationId, SectionId, "1", new List<Answer>
            {
                new Answer() {QuestionId = "Q1", Value = "No"}
            }), CancellationToken.None);

            var updatedSection = await DataContext.ApplicationSections.SingleAsync();

            updatedSection.QnAData.Pages.Single(page => page.PageId == "1").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "2").Active.Should().BeFalse();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "3").Active.Should().BeFalse();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "4").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "5").Active.Should().BeFalse();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "6").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "7").Active.Should().BeFalse();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "8").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "9").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "10").Active.Should().BeTrue();
        }
        
        [Test]
        public async Task Branch_in_a_branch_test()
        {
            await Handler.Handle(new SetPageAnswersRequest(ApplicationId, SectionId, "1", new List<Answer>
            {
                new Answer() {QuestionId = "Q1", Value = "Yes"}
            }), CancellationToken.None);
            
            await Handler.Handle(new SetPageAnswersRequest(ApplicationId, SectionId, "2", new List<Answer>
            {
                new Answer() {QuestionId = "Q2", Value = "Yes"}
            }), CancellationToken.None);

            var updatedSection = await DataContext.ApplicationSections.SingleAsync();

            updatedSection.QnAData.Pages.Single(page => page.PageId == "1").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "2").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "3").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "4").Active.Should().BeFalse();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "5").Active.Should().BeFalse();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "6").Active.Should().BeFalse();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "7").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "8").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "9").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "10").Active.Should().BeTrue();
        }
        
        [Test]
        public async Task Branch_in_a_branch_test_2()
        {
            await Handler.Handle(new SetPageAnswersRequest(ApplicationId, SectionId, "1", new List<Answer>
            {
                new Answer() {QuestionId = "Q1", Value = "Yes"}
            }), CancellationToken.None);
            
            await Handler.Handle(new SetPageAnswersRequest(ApplicationId, SectionId, "2", new List<Answer>
            {
                new Answer() {QuestionId = "Q2", Value = "No"}
            }), CancellationToken.None);

            var updatedSection = await DataContext.ApplicationSections.SingleAsync();

            updatedSection.QnAData.Pages.Single(page => page.PageId == "1").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "2").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "3").Active.Should().BeFalse();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "4").Active.Should().BeFalse();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "5").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "6").Active.Should().BeFalse();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "7").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "8").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "9").Active.Should().BeTrue();
            updatedSection.QnAData.Pages.Single(page => page.PageId == "10").Active.Should().BeTrue();
        }
    }
}