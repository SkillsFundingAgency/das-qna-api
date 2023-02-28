using SFA.DAS.QnA.Api.Types;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Api.Controllers
{
    public class HandlerResponseDeserializer
    {
        public async Task<object> Deserialize(HandlerResponse<string> handlerResponse){ return JsonSerializer.Deserialize<object>(handlerResponse.Value).ToString(); }
    }
}
