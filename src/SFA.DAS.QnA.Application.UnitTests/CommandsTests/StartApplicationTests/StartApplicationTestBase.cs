using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands.StartApplication;
using SFA.DAS.Qna.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.StartApplicationTests
{
    [TestFixture]
    public class StartApplicationTestBase
    {
        protected QnaDataContext DataContext;
        protected StartApplicationHandler Handler;
        protected Guid WorkflowId;

        [SetUp]
        public async Task SetUp()
        {
            DataContext = DataContextHelpers.GetInMemoryDataContext();
            
            Handler = new StartApplicationHandler(DataContext);

            WorkflowId = Guid.NewGuid();
            
            await DataContext.Workflows.AddAsync(new Workflow() {Type = "EPAO", Status = WorkflowStatus.Live, Id = WorkflowId});
            await DataContext.WorkflowSequences.AddRangeAsync(new[]
            {
                new WorkflowSequence {WorkflowId = WorkflowId}, 
                new WorkflowSequence {WorkflowId = WorkflowId}, 
                new WorkflowSequence {WorkflowId = Guid.NewGuid()}, 
                new WorkflowSequence {WorkflowId = Guid.NewGuid()}, 
            });
            await DataContext.WorkflowSections.AddRangeAsync(new[]
            {
                new WorkflowSection {WorkflowId = WorkflowId, Title = "Section 1"}, 
                new WorkflowSection {WorkflowId = WorkflowId, Title = "Section 2"}, 
                new WorkflowSection {WorkflowId = WorkflowId, Title = "Section 3"}, 
                new WorkflowSection {WorkflowId = WorkflowId, Title = "Section 4"}, 
                new WorkflowSection {WorkflowId = Guid.NewGuid(), Title = "Invalid section"}
            });
            await DataContext.SaveChangesAsync();
        }
    }
}