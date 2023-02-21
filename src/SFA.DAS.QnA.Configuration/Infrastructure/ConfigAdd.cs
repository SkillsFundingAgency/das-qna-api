using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SFA.DAS.QnA.Configuration.Infrastructure
{
    public static class ConfigHelper
    {
        public static IDictionary<string, string> AddKeyValuePairsToDictionary(JObject jsonObject, IDictionary<string, string> data)
        {
            foreach (var child in jsonObject.Children())
            {
                foreach (var jToken in child.Children().Children())
                {
                    var child1 = (JProperty)jToken;
                    data.Add($"{child.Path}:{child1.Name}", child1.Value.ToString());
                }
            }
            return data;
        }
    }
}
