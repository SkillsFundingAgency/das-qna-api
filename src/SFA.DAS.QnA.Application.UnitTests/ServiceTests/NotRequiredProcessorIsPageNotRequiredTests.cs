using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Services;

namespace SFA.DAS.QnA.Application.UnitTests.ServiceTests
{
    [TestFixture]
    public class NotRequiredProcessorIsPageNotRequiredTests
    {
        [TestCase("OrgType1", "OrgType1", true)]
        [TestCase("OrgType1", "ABCDEF", false)]
        [TestCase("OrgType1", "OrgType", false)]
        [TestCase("OrgType1", "orgType1", false)]
        [TestCase("OrgType1", null, false)]
        [TestCase("OrgType1", "", false)]
        [TestCase(null, null, true)]
        [TestCase(null, "", false)]
        [TestCase("", "", true)]
        [TestCase("", null, false)]  
        public void When_IsPageNotRequired_returns_expected_result_When_IsOneOf_specified(string notRequiredConditionValue, string applicationDataValue, bool expectedResult)
        {
            var applicationDataJson = JsonConvert.SerializeObject(new
            {
                FieldToTest = applicationDataValue
            });

            var applicationData = JObject.Parse(applicationDataJson);

            var page = new Page
            {
                PageId = "1",
                NotRequiredConditions = new List<NotRequiredCondition>
                {
                    new NotRequiredCondition
                    {
                        Field = "FieldToTest",
                        IsOneOf = new string[] { notRequiredConditionValue }
                    }
                }
            };

            var notRequiredProcessor = new NotRequiredProcessor();

            var result = notRequiredProcessor.IsPageNotRequired(page, applicationData);

            Assert.AreEqual(expectedResult, result);
        }

        [TestCase("OrgType1", "OrgType1", false)]
        [TestCase("OrgType1", "ABCDEF", true)]
        [TestCase("OrgType1", "OrgType", true)]
        [TestCase("OrgType1", "orgType1", true)]
        [TestCase("OrgType1", null, true)]
        [TestCase("OrgType1", "", true)]
        [TestCase(null, null, false)]
        [TestCase(null, "", true)]
        [TestCase("", "", false)]
        [TestCase("", null, true)]
        public void When_IsPageNotRequired_returns_expected_result_When_DoesNotContain_specified(string notRequiredConditionValue, string applicationDataValue, bool expectedResult)
        {
            var applicationDataJson = JsonConvert.SerializeObject(new
            {
                FieldToTest = applicationDataValue
            });

            var applicationData = JObject.Parse(applicationDataJson);

            var page = new Page
            {
                PageId = "1",
                NotRequiredConditions = new List<NotRequiredCondition>
                {
                    new NotRequiredCondition
                    {
                        Field = "FieldToTest",
                        DoesNotContain = new string[] { notRequiredConditionValue }
                    }
                }
            };

            var notRequiredProcessor = new NotRequiredProcessor();

            var result = notRequiredProcessor.IsPageNotRequired(page, applicationData);

            Assert.AreEqual(expectedResult, result);
        }

        [TestCase(new[] { "value1", "value2" }, "value1,value2", true)]
        [TestCase(new[] { "value1", "value2" }, "value2,value1", true)]
        [TestCase(new[] { "value1", "value2" }, "value1,value2,value3", true)]
        [TestCase(new[] { "value1", "value2" }, "value3,value2,value1", true)]
        [TestCase(new[] { "value1", "value2" }, "value4,value5", false)]
        [TestCase(new[] { "value1", "value2" }, "value1", false)]
        [TestCase(new[] { "value1", "value2" }, "", false)]
        [TestCase(new[] { "value1", "value2" }, null, false)]
        [TestCase(new[] { null, "value1" }, null, false)]
        [TestCase(new[] { null, "value1" }, "", false)]
        [TestCase(new string[] { null }, null, true)]
        [TestCase(new string[] { null }, "", false)]
        [TestCase(new string[] { "" }, "", true)]
        [TestCase(new string[] { "" }, null, false)]
        public void When_IsPageNotRequired_returns_expected_result_When_ContainsAllOf_specified(string[] containsAllValues, string applicationDataValue, bool expectedResult)
        {
            var applicationDataJson = JsonConvert.SerializeObject(new
            {
                FieldToTest = applicationDataValue
            });

            var applicationData = JObject.Parse(applicationDataJson);

            var page = new Page
            {
                PageId = "1",
                NotRequiredConditions = new List<NotRequiredCondition>
                {
                    new NotRequiredCondition
                    {
                        Field = "FieldToTest",
                        ContainsAllOf = containsAllValues
                    }
                }
            };

            var notRequiredProcessor = new NotRequiredProcessor();

            var result = notRequiredProcessor.IsPageNotRequired(page, applicationData);

            Assert.AreEqual(expectedResult, result);
        }
    }
}
