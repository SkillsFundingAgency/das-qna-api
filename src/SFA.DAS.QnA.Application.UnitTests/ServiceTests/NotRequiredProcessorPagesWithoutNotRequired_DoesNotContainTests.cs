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
    public class NotRequiredProcessorPagesWithoutNotRequired_DoesNotContainTests
    {
        [TestCase("OrgType1", "OrgType1", false)] // present so should not be removed
        [TestCase("OrgType1", "ABCDEF", true)]    // not present (no commonality) so should be removed
        [TestCase("OrgType1", "OrgType", true)]   // not present (partial match) so should be removed
        [TestCase("OrgType1", "orgType1", true)]  // not present (case sensitive) so should be removed
        [TestCase("OrgType1", "", true)]          // Empty application data should render as satisfied NRC
        [TestCase("OrgType1", null, true)]        // NULL application data should render as satisfied NRC
        [TestCase("", "", false)]                   // Emptiness check should render as unsatisfied NRC
        [TestCase(null, null, false)]               // NULL check should render as unsatisfied NRC
        public void When_DoesNotContain_conditions_are_removed_appropriately(string notRequiredConditionValue, string applicationDataValue, bool shouldRemovePage)
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
                            DoesNotContain = new string[] { notRequiredConditionValue }
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

        [TestCase("OrgType1")]
        [TestCase("")]
        [TestCase(null)]
        public void When_DoesNotContain_conditions_are_empty_then_page_is_removed(string applicationDataValue)
        {
            var applicationDataJson = JsonConvert.SerializeObject(new
            {
                FieldToTest = applicationDataValue
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
                            DoesNotContain = new string[] { }
                        }
                    }
                }
            };

            var notRequiredProcessor = new NotRequiredProcessor();
            var actualPages = notRequiredProcessor.PagesWithoutNotRequired(pages, applicationData);

            Assert.IsFalse(actualPages.Any());
        }

        [TestCase("OrgType1")]
        [TestCase("")]
        [TestCase(null)]
        public void When_DoesNotContain_conditions_are_null_then_page_remains(string applicationDataValue)
        {
            var applicationDataJson = JsonConvert.SerializeObject(new
            {
                FieldToTest = applicationDataValue
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
                            DoesNotContain = null
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

