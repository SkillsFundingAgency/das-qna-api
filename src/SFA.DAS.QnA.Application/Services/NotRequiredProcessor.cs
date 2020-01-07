using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Services
{
    public class NotRequiredProcessor: INotRequiredProcessor
    {
        public bool NotRequired(List<NotRequiredCondition> notRequiredConditions, JObject applicationData)
        {
            return notRequiredConditions.Any(nrc =>
                nrc.IsOneOf != null && nrc.IsOneOf.Contains(applicationData[nrc.Field]?.Value<string>()));
        }

        public List<Page> PagesWithoutNotRequired(List<Page> pages, JObject applicationData)
        {
            pages.RemoveAll(p => p.NotRequiredConditions != null &&
                                 p.NotRequiredConditions.Any(nrc => nrc.IsOneOf != null && nrc.IsOneOf.Contains(applicationData[nrc.Field]?.Value<string>())));


            var pagesProcessed = new List<Page>();

            foreach (var page in pages)
            {
                if (page.NotRequiredConditions==null)
                    pagesProcessed.Add(page);
                else
                {
                    var fieldMatch = false;
                    foreach (var notRequiredCondition in page.NotRequiredConditions)
                    {
                        var fieldToCheck = notRequiredCondition.Field;
                        var fieldValue = applicationData[fieldToCheck]?.Value<string>();
                        if (string.IsNullOrEmpty(fieldValue)) continue;

                            if (notRequiredCondition.ContainsAllOf != null)
                            {
                                fieldMatch = true;
                                foreach (var containsAllOf in notRequiredCondition.ContainsAllOf)
                                    if (!fieldValue.Contains(containsAllOf))
                                        fieldMatch = false;
                            }
                    }

                    if(!fieldMatch)
                        pagesProcessed.Add(page);

                }
            }

            return pagesProcessed;
        }
    }
}
