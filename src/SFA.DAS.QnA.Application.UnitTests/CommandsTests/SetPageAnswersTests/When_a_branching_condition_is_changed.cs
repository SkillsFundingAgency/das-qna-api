using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SetPageAnswersTests
{
    public class When_a_branching_condition_is_changed : SetPageAnswersTestBase
    {
        [Test]
        [Ignore("Need to look again at the test data")]
        public async Task Correct_old_decision_pages_are_disabled()
        {
            await Handler.Handle(new SetPageAnswersRequest(ApplicationId, SectionId, "1", new List<Answer>
            {
                new Answer() {QuestionId = "Q1", Value = "Yes"}
            }), CancellationToken.None);
            
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
        public async Task Correct_old_decision_pages_are_disabled_2()
        {
            await Handler.Handle(new SetPageAnswersRequest(ApplicationId, SectionId, "1", new List<Answer>
            {
                new Answer() {QuestionId = "Q1", Value = "Yes"}
            }), CancellationToken.None);
            
            await Handler.Handle(new SetPageAnswersRequest(ApplicationId, SectionId, "2", new List<Answer>
            {
                new Answer() {QuestionId = "Q2", Value = "No"}
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
    }
}