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
    public class NotRequiredProcessorPagesWithoutNotRequired_IsOneOfTests
    {
        [TestCase("OrgType1", "OrgType1", true)]   // present so should be removed
        [TestCase("OrgType1", "ABCDEF", false)]    // not present (no commonality) so should not be removed
        [TestCase("OrgType1", "OrgType", false)]   // not present (partial match) so should not be removed
        [TestCase("OrgType1", "orgType1", false)]  // not present (case sensitive) so should not be removed
        [TestCase("OrgType1", "", false)]          // Empty application data should render as unsatisfied NRC
        [TestCase("OrgType1", null, false)]        // NULL application data should render as unsatisfied NRC
        public void When_IsOneOf_conditions_are_removed_appropriately(string notRequiredConditionValue, string applicationDataValue, bool shouldRemovePage)
        {
            var expectedPagesCount = shouldRemovePage ? 1 : 2;
            var pageIdAbsentIfNotRequired = "2";
            var pageIdAlwaysPresent = "3";

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
                            IsOneOf = new string[] { notRequiredConditionValue }
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

            Assert.AreEqual(actualPages.ToList().Count, expectedPagesCount);
            Assert.IsTrue(actualPages.Any(p => p.PageId == pageIdAlwaysPresent));
            Assert.AreNotEqual(actualPages.Any(p => p.PageId == pageIdAbsentIfNotRequired), shouldRemovePage);
        }

        [Test]
        public void When_IsOneOf_conditions_are_empty_then_page_remains()
        {
            var applicationDataJson = JsonConvert.SerializeObject(new
            {
                FieldToTest = "OrgType1"
            });

            var applicationData = JObject.Parse(applicationDataJson);

            var pages = new List<Page>
            {
                new Page
                {
                    PageId = "2",
                    NotRequiredConditions = new List<NotRequiredCondition>
                    {
                        new NotRequiredCondition()
                        {
                            Field = "FieldToTest",
                            IsOneOf = new string[] { }
                        }
                    }
                }
            };

            var notRequiredProcessor = new NotRequiredProcessor();
            var actualPages = notRequiredProcessor.PagesWithoutNotRequired(pages, applicationData);

            Assert.IsTrue(actualPages.Any());
        }

        [Test]
        public void When_IsOneOf_conditions_are_null_then_page_remains()
        {
            var applicationDataJson = JsonConvert.SerializeObject(new
            {
                FieldToTest = "OrgType1"
            });

            var applicationData = JObject.Parse(applicationDataJson);

            var pages = new List<Page>
            {
                new Page
                {
                    PageId = "2",
                    NotRequiredConditions = new List<NotRequiredCondition>
                    {
                        new NotRequiredCondition()
                        {
                            Field = "FieldToTest",
                            IsOneOf = null
                        }
                    }
                }
            };

            var notRequiredProcessor = new NotRequiredProcessor();
            var actualPages = notRequiredProcessor.PagesWithoutNotRequired(pages, applicationData);

            Assert.IsTrue(actualPages.Any());
        }
    }
}

