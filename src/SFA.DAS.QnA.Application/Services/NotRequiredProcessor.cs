using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Services
{
    public class NotRequiredProcessor: INotRequiredProcessor
    {
        public bool IsPageNotRequired(Page page, JObject applicationData)
        {
            if (page?.NotRequiredConditions is null || applicationData is null) return false;

            return GetMatchingConditions_IsOneOf(page.NotRequiredConditions, applicationData).Any()
                || GetMatchingConditions_ContainsAllOf(page.NotRequiredConditions, applicationData).Any()
                || GetMatchingConditions_DoesNotContain(page.NotRequiredConditions, applicationData).Any();
        }

        public IEnumerable<Page> PagesWithoutNotRequired(List<Page> pages, JObject applicationData)
        {
            return pages?.Where(p => IsPageNotRequired(p, applicationData) is false);
        }

        private static IEnumerable<NotRequiredCondition> GetMatchingConditions_IsOneOf(List<NotRequiredCondition> notRequiredConditions, JObject applicationData)
        {
            var matchingConditions = new List<NotRequiredCondition>();

            foreach (var notRequiredCondition in notRequiredConditions.Where(n => n.IsOneOf != null))
            {
                var applicationDataValues = applicationData[notRequiredCondition.Field]?.Value<string>().Split(",", StringSplitOptions.RemoveEmptyEntries).Distinct();

                if (applicationDataValues is null) continue;

                var valuesThatMatch = notRequiredCondition.IsOneOf.Intersect(applicationDataValues);

                if (valuesThatMatch.Any())
                {
                    matchingConditions.Add(notRequiredCondition);
                }
            }

            return matchingConditions;
        }

        private static IEnumerable<NotRequiredCondition> GetMatchingConditions_ContainsAllOf(List<NotRequiredCondition> notRequiredConditions, JObject applicationData)
        {
            var matchingConditions = new List<NotRequiredCondition>();

            foreach (var notRequiredCondition in notRequiredConditions.Where(n => n.ContainsAllOf != null))
            {
                var applicationDataValues = applicationData[notRequiredCondition.Field]?.Value<string>().Split(",", StringSplitOptions.RemoveEmptyEntries).Distinct();

                if (applicationDataValues is null) continue;

                var valuesThatDidNotMatch = notRequiredCondition.ContainsAllOf.Except(applicationDataValues);

                if (!valuesThatDidNotMatch.Any())
                {
                    matchingConditions.Add(notRequiredCondition);
                }
            }

            return matchingConditions;
        }

        private static IEnumerable<NotRequiredCondition> GetMatchingConditions_DoesNotContain(List<NotRequiredCondition> notRequiredConditions, JObject applicationData)
        {
            var matchingConditions = new List<NotRequiredCondition>();

            foreach (var notRequiredCondition in notRequiredConditions.Where(n => n.DoesNotContain != null))
            {
                var applicationDataValues = applicationData[notRequiredCondition.Field]?.Value<string>().Split(",", StringSplitOptions.RemoveEmptyEntries).Distinct();

                if (applicationDataValues is null) continue;

                var valuesThatMatch = notRequiredCondition.DoesNotContain.Intersect(applicationDataValues);

                if (!valuesThatMatch.Any())
                {
                    matchingConditions.Add(notRequiredCondition);
                }
            }

            return matchingConditions;
        }
    }
}
