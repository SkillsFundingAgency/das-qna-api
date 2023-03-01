using Newtonsoft.Json;

namespace SFA.DAS.QnA.Api.Types.Page
{
    internal class ModelValueDeserialiser
    {
        internal static dynamic Deserialize(string modelValue)
        {
            return JsonConvert.DeserializeObject<dynamic>(modelValue); // the old code uses Newtonsoft
        }
    }
}
