using Azure.Core;
using Azure.Identity;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Api.Infrastructure
{
    public static class SqlTokenGenerator
    {
        private const string AzureResource = "https://database.windows.net/";

        public static async Task<string> GenerateTokenAsync()
        {
            var credential = new DefaultAzureCredential();

            var tokenRequestContext = new TokenRequestContext([AzureResource]);
            var accessToken = await credential.GetTokenAsync(tokenRequestContext);

            return accessToken.Token;
        }
    }
}
