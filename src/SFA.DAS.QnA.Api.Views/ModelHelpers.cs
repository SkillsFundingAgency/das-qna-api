using System.Collections.Generic;
using System.Dynamic;

namespace SFA.DAS.QnA.Api.Views
{
    public class ModelHelpers
    {
        public static bool PropertyExists(dynamic dynamicObject, string name)
        {
            if (dynamicObject is ExpandoObject)
                return ((IDictionary<string, object>)dynamicObject).ContainsKey(name);

            return dynamicObject.GetType().GetProperty(name) != null;
        }
    }
}