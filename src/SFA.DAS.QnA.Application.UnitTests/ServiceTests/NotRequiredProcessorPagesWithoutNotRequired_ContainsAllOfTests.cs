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
    public class NotRequiredProcessorPagesWithoutNotRequired_ContainsAllOfTests
    {
        [TestCase(new[] { "value1", "value2" }, "value1,value2", true)] // contains all so should be removed
        [TestCase(new[] { "value1", "value2" }, "value2,value1", true)] // contains all (application data flipped) so should be removed
        [TestCase(new[] { "value2", "value1" }, "value1,value2", true)] // contains all (conditions flipped) so should be removed
        [TestCase(new[] { "value1", "value2" }, "value1,value2,value3", true)] // contains all (application data is a superset) so should be removed
        [TestCase(new[] { "value1", "value2" }, "value1,value3,value2", true)] // contains all (application data is a superset and not in same order) so should be removed
        [TestCase(new[] { "value1", "value2" }, "value3,value1,value2", true)] // contains all (application data is a superset and not in same order) so should be removed
        [TestCase(new[] { "value1", "value2" }, "value3,value2,value1", true)] // contains all (application data is a superset and in reverse order) so should be removed
        [TestCase(new[] { "value1", "value2" }, "value3", false)] // Does not contain any so should not be removed
        [TestCase(new[] { "value1", "value2" }, "value4,value5", false)] // Does not contain any so should not be removed
        [TestCase(new[] { "value1", "value2" }, "value1", false)] // Only contains 1 (first) match so should not be removed
        [TestCase(new[] { "value1", "value2" }, "value2", false)] // Only contains 1 (last) match so should not be removed
        [TestCase(new[] { "value1", "value2" }, "", false)] // Empty application data should render as unsatisfied NRC
        [TestCase(new[] { "value1", "value2" }, null, false)] // NULL application data should render as unsatisfied NRC
        [TestCase(new string[] { null }, null, true)] // NULL check should render as satisfied NRC
        [TestCase(new string[] { null }, "", false)] // NULL check with Empty application data should render as unsatisfied NRC
        [TestCase(new string[] { "" }, "", true)] // Emptiness check should render as satisfied NRC
        [TestCase(new string[] { "" }, null, false)] // Emptiness check will NULL application data should render as unsatisfied NRC
        public void When_PagesWithNotRequired_conditions_with_containsAllOf(string[] containsAllValues, string applicationDataValue, bool shouldRemovePage)
        {
            var expectedPagesCount = shouldRemovePage ? 1 : 2;
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
                        new NotRequiredCondition
                        {
                            Field = "FieldToTest",
                            ContainsAllOf = containsAllValues
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

            Assert.AreEqual(actualPages.Count(), expectedPagesCount);
            Assert.IsTrue(actualPages.Any(p => p.PageId == pageIdAlwaysPresent));
            Assert.AreNotEqual(actualPages.Any(p => p.PageId == pageIdAbsentIfNotRequired), shouldRemovePage);
        }

        [TestCase("OrgType1")]
        [TestCase("")]
        [TestCase(null)]
        public void When_ContainsAllOf_conditions_are_empty_then_page_is_removed(string applicationDataValue)
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
                            ContainsAllOf = new string[] { }
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
        public void When_ContainsAllOf_conditions_are_null_then_page_remains(string applicationDataValue)
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
                            ContainsAllOf = null
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

