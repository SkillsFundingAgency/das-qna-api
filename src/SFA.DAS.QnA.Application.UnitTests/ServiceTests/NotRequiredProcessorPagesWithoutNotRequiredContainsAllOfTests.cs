using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Services;

namespace SFA.DAS.QnA.Application.UnitTests.ServiceTests
{
    [TestFixture]
    public class NotRequiredProcessorPagesWithoutNotRequiredContainsAllOfTests
    {
            //[TestCase("OrgType1", "OrgType1", true)]
            //[TestCase("OrgType2", "OrgType2", true)]
            //[TestCase("OrgType2", "OrgType1", false)]
            //[TestCase("OrgType1", "OrgType2", false)]
            //[TestCase("OrgType1", "orgType1", false)]
            //[TestCase("orgType1", "OrgType1", false)]
            //[TestCase("rgType1", "OrgType1", false)]
            //[TestCase("OrgType1", "rgType1", false)]

            //public void When_PagesWithNotRequired_conditions_are_removed(string notRequiredConditionValue, string applicationDataValue, bool match)
            [Test]
            public void When_PagesWithNotRequired_conditions_with_containsAllOf()
            {
                var applicationDataValue = "value3,value1,value2";
                var expectedPagesCount = 1;
                var match = true;
          
                var pageIdAlwaysPresent = "3";
                var pageIdAbsentIfNotRequired = "2";
                var applicationDataJson = JsonConvert.SerializeObject(new
                {
                    FieldToTest = applicationDataValue
                });

                var applicationData = JObject.Parse(applicationDataJson);

                var pages = new List<Page>
            {
                new Page
                {
                    PageId = pageIdAbsentIfNotRequired,
                    NotRequiredConditions = new List<NotRequiredCondition>
                    {
                        new NotRequiredCondition()
                        {
                            Field = "FieldToTest",
                            ContainsAllOf = new string[] {"value1", "value2"}
                        }
                    },
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
                        }
                    }
                },
                new Page
                {
                    PageId = pageIdAlwaysPresent,
                    NotRequiredConditions = null
                }
            };

                var notRequiredProcessor = new NotRequiredProcessor();
                var actualPages = notRequiredProcessor.PagesWithoutNotRequired(pages, applicationData);

                Assert.AreEqual(actualPages.Count, expectedPagesCount);
                //Assert.IsTrue(actualPages.Any(p => p.PageId == pageIdAlwaysPresent));
                //Assert.AreNotEqual(actualPages.Any(p => p.PageId == pageIdAbsentIfNotRequired), match);
            }
        }
    }

