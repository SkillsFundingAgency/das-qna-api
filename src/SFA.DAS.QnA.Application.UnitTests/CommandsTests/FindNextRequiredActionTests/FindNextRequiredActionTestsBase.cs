using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.FindNextRequiredActionTests
{
    [TestFixture]
    public class FindNextRequiredActionTestsBase
    {
        protected SetAnswersBase SetAnswersBase;
        protected QnaDataContext QnaDataContext;
        protected Guid ApplicationId;
        protected Next NextAction;
        protected string ApplicationDataJson;
        protected INotRequiredProcessor notRequiredProcessor;

        [SetUp]
        public async Task SetUp()
        {
            QnaDataContext = DataContextHelpers.GetInMemoryDataContext();

            notRequiredProcessor = new NotRequiredProcessor();
            SetAnswersBase = new SetAnswersBase(QnaDataContext, notRequiredProcessor);

            ApplicationId = Guid.NewGuid();

            ApplicationDataJson = JsonConvert.SerializeObject(new
            {
                OrgType = "OrgType1"
            });

            await QnaDataContext.Applications.AddAsync(new Data.Entities.Application { Id = ApplicationId, ApplicationData = ApplicationDataJson });

            await QnaDataContext.SaveChangesAsync();

            NextAction = new Next {Action = "NextPage", ReturnId = "2"};   
        }


        [Test]
        public void TestNoNextAction()
        {
            var applicationSection = new ApplicationSection();
            Next nextAction = null;
             var applicationData = JObject.Parse("{}");
            var result = SetAnswersBase.FindNextRequiredAction(applicationSection, nextAction, applicationData);

            Assert.IsNull(result);
        }
    }
}