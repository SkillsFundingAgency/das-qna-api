using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.FindNextRequiredActionTests
{
    [TestFixture]
    public class FindNextRequiredActionTestsBase
    {
        protected SetAnswersBase SetAnswersBase;
        protected QnaDataContext QnaDataContext;
        protected Guid ApplicationId;
        protected Next NextAction;

        [SetUp]
        public async Task SetUp()
        {
            SetAnswersBase = new SetAnswersBase();

            QnaDataContext = DataContextHelpers.GetInMemoryDataContext();

            ApplicationId = Guid.NewGuid();
            
            await QnaDataContext.Applications.AddAsync(new Data.Entities.Application {Id = ApplicationId, ApplicationData = JsonConvert.SerializeObject(new
            {
                OrgType = "HEI"
            })});

            await QnaDataContext.SaveChangesAsync();

            NextAction = new Next {Action = "NextPage", ReturnId = "2"};   
        }
    }
}