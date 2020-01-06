using System;
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
                                 p.NotRequiredConditions.Any(nrc => nrc.IsOneOf.Contains(applicationData[nrc.Field]?.Value<string>())));

            return pages;
        }
    }
}
