using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.FindNextRequiredActionTests
{
    public class When_multiple_next_actions_and_page_is_not_required : FindNextRequiredActionTestsBase
    {

        [Test]
        public void And_all_of_the_nextactions_have_a_condition_The_the_last_action_is_returned()
        {
            var expectedNextAction = new Next
            {
                Action = "NextPage",
                ReturnId = "3",
                Conditions = new List<Condition>()
            };
            
            var section = new ApplicationSection
            {
                ApplicationId = ApplicationId,
                QnAData = new QnAData {Pages = new List<Page>
                {
                    new Page
                    {
                        PageId = "2",
                        NotRequiredConditions = new List<NotRequiredCondition>{new NotRequiredCondition(){Field = "OrgType", IsOneOf = new string[]{"HEI","OrgType2"}}},
                        Next = new List<Next>
                        {
                            new Next
                            {
                                Action = "NextPage",
                                ReturnId = "12",
                                Conditions = new List<Condition>()
                            },
                            new Next
                            {
                                Action = "NextPage",
                                ReturnId = "14",
                                Conditions = new List<Condition>() 
                            },
                            expectedNextAction
                        }
                    },
                    new Page
                    {
                        PageId = "3",
                        NotRequiredConditions = null
                    }
                }}
            };
            
            var nextActionAfterFindingNextAction = SetAnswersBase.FindNextRequiredAction(section, QnaDataContext, NextAction);
            nextActionAfterFindingNextAction.Should().BeEquivalentTo(expectedNextAction);
        }
        
        [Test]
        public void And_one_of_the_nextactions_has_condition_null_Then_that_action_is_returned()
        {
            var actionWithNoCondition = new Next
            {
                Action = "NextPage",
                ReturnId = "3",
                Conditions = null
            };
            
            var section = new ApplicationSection
            {
                ApplicationId = ApplicationId,
                QnAData = new QnAData {Pages = new List<Page>
                {
                    new Page
                    {
                        PageId = "2",
                        NotRequiredConditions = new List<NotRequiredCondition>{new NotRequiredCondition(){Field = "OrgType", IsOneOf = new string[]{ "HEI", "OrgType2"}}},
                        Next = new List<Next>
                        {
                            new Next
                            {
                                Action = "NextPage",
                                ReturnId = "12",
                                Conditions = new List<Condition>()
                            },
                            actionWithNoCondition,
                            new Next
                            {
                                Action = "NextPage",
                                ReturnId = "14",
                                Conditions = new List<Condition>() 
                            }
                        }
                    },
                    new Page
                    {
                        PageId = "3",
                        NotRequiredConditions = null
                    }
                }}
            };
            
            var nextActionAfterFindingNextAction = SetAnswersBase.FindNextRequiredAction(section, QnaDataContext, NextAction);
            nextActionAfterFindingNextAction.Should().BeEquivalentTo(actionWithNoCondition);
        }
    }



    public class When_next_page_has_a_NotRequiredCondition : FindNextRequiredActionTestsBase
    {
        [TestCase("HEI", null, "3")]
        [TestCase("TEST", null, "2")]
        [TestCase(null, "HEI", "2")]
        [TestCase(null, "TEST", "3")]
        [TestCase("TEST", "HEI", "2")]
        [TestCase("HEI", "TEST", "3")]
        [TestCase("HEI", "HEI", "3")]
        [TestCase("TEST", "TEST", "3")]
        public void Subsequent_nextAction_is_returned(string oneOf, string notOneOf, string expectedReturnId)
        {
            var isOneOf = oneOf == null ? null : new string[] { oneOf };
            var isNotOneOf = notOneOf == null ? null : new string[] { notOneOf };

            var expectedNextAction = new Next
            {
                Action = "NextPage",
                ReturnId = expectedReturnId
            };
            
            var section = new ApplicationSection
            {
                ApplicationId = ApplicationId,
                QnAData = new QnAData {Pages = new List<Page>
                {
                    new Page
                    {
                        PageId = "2",
                        NotRequiredConditions = new List<NotRequiredCondition>{new NotRequiredCondition(){Field = "OrgType", IsOneOf = isOneOf, IsNotOneOf = isNotOneOf} },
                        Next = new List<Next>{new Next {Action = "NextPage", ReturnId = "3"}}
                    },
                    new Page
                    {
                        PageId = "3",
                        NotRequiredConditions = null
                    }
                }}
            };
            
            var nextActionAfterFindingNextAction = SetAnswersBase.FindNextRequiredAction(section, QnaDataContext, NextAction);
            nextActionAfterFindingNextAction.Should().BeEquivalentTo(expectedNextAction);
        }

        [TestCase("HEI", null, "HEI", null, "4")]
        [TestCase("HEI", null, "TEST", null, "3")]
        [TestCase("TEST", null, "HEI", null, "2")]
        [TestCase("TEST", null, "TEST", null, "2")]
        [TestCase(null, "HEI", null, "HEI", "2")]
        [TestCase(null, "HEI", null, "TEST", "2")]
        [TestCase(null, "TEST", null, "HEI", "3")]
        [TestCase(null, "TEST", null, "TEST", "4")]
        [TestCase("HEI", "TEST", "HEI", "TEST", "4")]
        [TestCase("HEI", "TEST", "TEST", "HEI", "3")]
        [TestCase("TEST", "HEI", "TEST", "HEI", "2")]
        [TestCase("TEST", "HEI", "HEI", "TEST", "2")]
        [TestCase("HEI", "HEI", "HEI", "HEI", "4")]
        [TestCase("TEST", "TEST", "TEST", "TEST", "4")]
        public void Subsequent_nextAction_further_down_the_branch_is_returned(string page2OneOf, string page2NotOneOf, string page3OneOf, string page3NotOneOf, string expectedReturnId)
        {
            var page2IsOneOf = page2OneOf == null ? null : new string[] { page2OneOf };
            var page2IsNotOneOf = page2NotOneOf == null ? null : new string[] { page2NotOneOf };
            var page3IsOneOf = page3OneOf == null ? null : new string[] { page3OneOf };
            var page3IsNotOneOf = page3NotOneOf == null ? null : new string[] { page3NotOneOf };

            var expectedNextAction = new Next
            {
                Action = "NextPage",
                ReturnId = expectedReturnId
            };
            
            var section = new ApplicationSection
            {
                ApplicationId = ApplicationId,
                QnAData = new QnAData {Pages = new List<Page>
                {
                    new Page
                    {
                        PageId = "2",
                        NotRequiredConditions = new List<NotRequiredCondition>{new NotRequiredCondition(){Field = "OrgType", IsOneOf = page2IsOneOf, IsNotOneOf = page2IsNotOneOf } },
                        Next = new List<Next>{new Next{Action = "NextPage", ReturnId = "3"}}
                    },
                    new Page
                    {
                        PageId = "3",
                        NotRequiredConditions = new List<NotRequiredCondition>{new NotRequiredCondition(){Field = "OrgType", IsOneOf = page3IsOneOf, IsNotOneOf = page3IsNotOneOf } },
                        Next = new List<Next>{new Next{Action = "NextPage", ReturnId = "4"}}
                    },
                    new Page
                    {
                        PageId = "4",
                        NotRequiredConditions = null
                    }
                }}
            };
            
            var nextActionAfterFindingNextAction = SetAnswersBase.FindNextRequiredAction(section, QnaDataContext, NextAction);
            nextActionAfterFindingNextAction.Should().BeEquivalentTo(expectedNextAction);
        }
    }
}