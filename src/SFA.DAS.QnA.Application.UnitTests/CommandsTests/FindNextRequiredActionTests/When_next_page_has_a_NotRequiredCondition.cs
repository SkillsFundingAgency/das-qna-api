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
                Condition = new Condition()
            };
            
            var section = new ApplicationSection
            {
                ApplicationId = ApplicationId,
                QnAData = new QnAData {Pages = new List<Page>
                {
                    new Page
                    {
                        PageId = "2",
                        NotRequiredConditions = new List<NotRequiredCondition>{new NotRequiredCondition(){Field = "OrgType", IsOneOf = new string[]{"OrgType1","OrgType2"}}},
                        Next = new List<Next>
                        {
                            new Next
                            {
                                Action = "NextPage",
                                ReturnId = "12",
                                Condition = new Condition()
                            },
                            new Next
                            {
                                Action = "NextPage",
                                ReturnId = "14",
                                Condition = new Condition()
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
                ReturnId = "3"
            };
            
            var section = new ApplicationSection
            {
                ApplicationId = ApplicationId,
                QnAData = new QnAData {Pages = new List<Page>
                {
                    new Page
                    {
                        PageId = "2",
                        NotRequiredConditions = new List<NotRequiredCondition>{new NotRequiredCondition(){Field = "OrgType", IsOneOf = new string[]{"OrgType1","OrgType2"}}},
                        Next = new List<Next>
                        {
                            new Next
                            {
                                Action = "NextPage",
                                ReturnId = "12",
                                Condition = new Condition()
                            },
                            actionWithNoCondition,
                            new Next
                            {
                                Action = "NextPage",
                                ReturnId = "14",
                                Condition = new Condition()
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
        [Test]
        public async Task Subsequent_nextAction_is_returned()
        {
            var pageTwoNextAction = new Next
            {
                Action = "NextPage",
                ReturnId = "3"
            };
            
            var section = new ApplicationSection
            {
                ApplicationId = ApplicationId,
                QnAData = new QnAData {Pages = new List<Page>
                {
                    new Page
                    {
                        PageId = "2",
                        NotRequiredConditions = new List<NotRequiredCondition>{new NotRequiredCondition(){Field = "OrgType", IsOneOf = new string[]{"OrgType1","OrgType2"}}},
                        Next = new List<Next>{pageTwoNextAction}
                    },
                    new Page
                    {
                        PageId = "3",
                        NotRequiredConditions = null
                    }
                }}
            };
            
            var nextActionAfterFindingNextAction = SetAnswersBase.FindNextRequiredAction(section, QnaDataContext, NextAction);
            nextActionAfterFindingNextAction.Should().BeEquivalentTo(pageTwoNextAction);
        }
        
        [Test]
        public async Task Subsequent_nextAction_further_down_the_branch_is_returned()
        {
            var pageThreeNextAction = new Next
            {
                Action = "NextPage",
                ReturnId = "4"
            };
            
            var section = new ApplicationSection
            {
                ApplicationId = ApplicationId,
                QnAData = new QnAData {Pages = new List<Page>
                {
                    new Page
                    {
                        PageId = "2",
                        NotRequiredConditions = new List<NotRequiredCondition>{new NotRequiredCondition(){Field = "OrgType", IsOneOf = new string[]{"OrgType1","OrgType2"}}},
                        Next = new List<Next>{new Next{Action = "NextPage",
                            ReturnId = "3"}}
                    },
                    new Page
                    {
                        PageId = "3",
                        NotRequiredConditions = new List<NotRequiredCondition>{new NotRequiredCondition(){Field = "OrgType", IsOneOf = new string[]{"OrgType1"}}},
                        Next = new List<Next>{pageThreeNextAction}
                    },
                    new Page
                    {
                        PageId = "4",
                        NotRequiredConditions = null
                    }
                }}
            };
            
            var nextActionAfterFindingNextAction = SetAnswersBase.FindNextRequiredAction(section, QnaDataContext, NextAction);
            nextActionAfterFindingNextAction.Should().BeEquivalentTo(pageThreeNextAction);
        }
    }
}