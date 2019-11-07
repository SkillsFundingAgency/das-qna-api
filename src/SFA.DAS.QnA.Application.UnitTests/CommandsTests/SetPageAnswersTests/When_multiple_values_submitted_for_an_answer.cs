using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SetPageAnswersTests
{
    [TestFixture]
    public class When_multiple_values_submitted_for_an_answer
    {
        [Test]
        public async Task Then_ApplicationData_is_updated_with_array_of_values()
        {
            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();

            var dataContext = DataContextHelpers.GetInMemoryDataContext();

            await dataContext.Applications.AddAsync(new Data.Entities.Application(){Id = applicationId, ApplicationData = "{}"});
            
            await dataContext.ApplicationSections.AddAsync(new ApplicationSection()
            {
                ApplicationId = applicationId, Id = sectionId, 
                QnAData = new QnAData()
                {
                    Pages = new List<Page>()
                    {
                        new Page()
                        {
                            PageId = "1",
                            Questions = new List<Question>{new Question(){QuestionId = "Q1", Input = new Input(), QuestionTag = "q1tag"}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "NextPage", ReturnId = "2", Conditions = new List<Condition>()}
                            },
                            Active = true
                        },
                    }
                }
            });
            
            await dataContext.SaveChangesAsync();
            
            var answerValidator = Substitute.For<IAnswerValidator>();

            var answers = new List<Answer>()
            {
                new Answer(){QuestionId = "Q1", Value = new []{"Blue", "Red"}}
            };

            answerValidator.Validate(answers, Arg.Any<Page>()).Returns(new List<KeyValuePair<string, string>>());
            
            var setPageAnswersHandler = new SetPageAnswersHandler(dataContext, answerValidator);

            
            await setPageAnswersHandler.Handle(new SetPageAnswersRequest(applicationId, sectionId, "1", answers), new CancellationToken());


            var application = dataContext.Applications.Single();

            application.ApplicationData.Should().NotBeEmpty();

            var appData = JObject.Parse(application.ApplicationData);

            var applicationQiTag = appData["q1tag"].Value<JArray>();

            applicationQiTag.Count.Should().Be(2);
            applicationQiTag[0].Value<string>().Should().Be("Blue");
            applicationQiTag[1].Value<string>().Should().Be("Red");
        }
    }
}