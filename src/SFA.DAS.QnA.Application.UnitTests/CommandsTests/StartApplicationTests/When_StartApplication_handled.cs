using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands.StartApplication;
using SFA.DAS.Qna.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.StartApplicationTests
{
    public class When_StartApplication_handled : StartApplicationTestBase
    {
        [Test]
        public async Task Then_a_new_Application_record_is_created()
        {
            await Handler.Handle(new StartApplicationRequest() {UserReference = "dave", WorkflowType = "EPAO"}, CancellationToken.None);

            var newApplications = await DataContext.Applications.ToListAsync();

            newApplications.Count.Should().Be(1);
            newApplications.First().CreatedFromWorkflowId.Should().Be(WorkflowId);
            newApplications.First().ApplicationStatus.Should().Be(ApplicationStatus.InProgress);
            newApplications.First().CreatedBy.Should().Be("dave");
        }
        
        [Test]
        public async Task Then_the_correct_sequences_are_created()
        {
            await Handler.Handle(new StartApplicationRequest() {UserReference = "dave", WorkflowType = "EPAO"}, CancellationToken.None);

            var newApplication = await DataContext.Applications.FirstAsync();
            var newSequences = await DataContext.ApplicationSequences.ToListAsync();

            newSequences.Count.Should().Be(2);
            newSequences.Should().AllBeEquivalentTo(new{ApplicationId = newApplication.Id});
        }
        
        [Test]
        public async Task Then_the_correct_sections_are_created()
        {
            await Handler.Handle(new StartApplicationRequest() {UserReference = "dave", WorkflowType = "EPAO"}, CancellationToken.None);

            var newApplication = await DataContext.Applications.FirstAsync();
            var newSections = await DataContext.ApplicationSections.ToListAsync();

            newSections.Count.Should().Be(4);
            newSections.Should().AllBeEquivalentTo(new{ApplicationId = newApplication.Id});
            newSections[0].Title.Should().Be("Section 1");
            newSections[1].Title.Should().Be("Section 2");
            newSections[2].Title.Should().Be("Section 3");
            newSections[3].Title.Should().Be("Section 4");
        }

        [Test]
        public async Task Then_QnAData_is_updated_from_assets()
        {
            DataContext.WorkflowSections.RemoveRange(DataContext.WorkflowSections);
            await DataContext.WorkflowSections.AddAsync(new WorkflowSection()
            {
               WorkflowId = WorkflowId,
               QnAData = new QnAData(){Pages = new List<Page>()
               {
                   new Page() {Title = "[PageTitleToken1]"},
                   new Page() {Title = "[PageTitleToken2]"}
               }}
            });
            
            await DataContext.Assets.AddRangeAsync(new[]
            {
                new Asset(){Reference = "[PageTitleToken1]", Text = "Page 1"}, 
                new Asset(){Reference = "[PageTitleToken2]", Text = "Page 2"} 
            });

            await DataContext.SaveChangesAsync();
            
            await Handler.Handle(new StartApplicationRequest() {UserReference = "dave", WorkflowType = "EPAO"}, CancellationToken.None);
            var newSections = await DataContext.ApplicationSections.ToListAsync();

            newSections[0].QnAData.Pages[0].Title.Should().Be("Page 1");
            newSections[0].QnAData.Pages[1].Title.Should().Be("Page 2");
        }
    }
}