using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Services
{
    public interface INotRequiredProcessor
    {
        bool IsPageNotRequired(Page page, JObject applicationData);
        IEnumerable<Page> PagesWithoutNotRequired(List<Page> pages, JObject applicationData);

    }
}
