﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.ResetPageAnswers;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;
using SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData;
using SFA.DAS.QnA.Application.Queries.Sections.GetPage;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.ResetPageAnswersTests
{
    [TestFixture]
    public class ResetPageAnswersTestBase
    {
        protected Guid ApplicationId;
        protected Guid SectionId;
        protected ResetPageAnswersHandler Handler;
        protected GetApplicationDataHandler GetApplicationDataHandler;
        protected GetPageHandler GetPageHandler;
        protected QnaDataContext DataContext;

        [SetUp]
        public async Task SetUp()
        {
            DataContext = DataContextHelpers.GetInMemoryDataContext();
            Handler = new ResetPageAnswersHandler(DataContext);
            GetApplicationDataHandler = new GetApplicationDataHandler(DataContext);
            GetPageHandler = new GetPageHandler(DataContext);

            ApplicationId = Guid.NewGuid();
            SectionId = Guid.NewGuid();
            await DataContext.ApplicationSections.AddAsync(new ApplicationSection()
            {
                ApplicationId = ApplicationId,
                Id = SectionId,
                QnAData = new QnAData()
                {
                    Pages = new List<Page>
                    {
                        new Page()
                        {
                            PageId = "1",
                            Questions = new List<Question>{new Question(){QuestionId = "Q1", QuestionTag = "Q1", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>{ new PageOfAnswers { Answers = new List<Answer> { new Answer { QuestionId = "Q1", Value = "Yes" } } } },
                            Next = new List<Next>
                            {
                                new Next(){Action = "NextPage", ReturnId = "2", Conditions = new List<Condition>(){ new Condition{QuestionId = "Q1", MustEqual = "Yes"}}},
                                new Next(){Action = "NextPage", ReturnId = "3", Conditions = new List<Condition>(){ new Condition{QuestionId = "Q1", MustEqual = "No"}}}
                            },
                            Feedback = new List<Feedback>{ new Feedback { IsCompleted = true } },
                            Active = true,
                            Complete = true
                        },
                        new Page()
                        {
                            PageId = "2",
                            Questions = new List<Question>{new Question(){QuestionId = "Q2", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "ReturnToSequence", Conditions = new List<Condition>() },
                            },
                            Active = true,
                            ActivatedByPageId = "1"
                        },
                        new Page()
                        {
                            PageId = "3",
                            Questions = new List<Question>{new Question(){QuestionId = "Q3", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "ReturnToSequence", Conditions = new List<Condition>()}
                            },
                            Active = false,
                            ActivatedByPageId = "1"
                        }
                    }
                }
            });

            await DataContext.Applications.AddAsync(new Data.Entities.Application() { Id = ApplicationId, ApplicationData = "{ \"Q1\" : \"Yes\" }" });

            await DataContext.SaveChangesAsync();
        }
    }
}
