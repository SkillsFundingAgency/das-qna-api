using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Api.Client
{
    public class QnaApiClient
    {
        private readonly HttpClient _httpClient;

        public QnaApiClient(HttpClient httpClient, QnaApiConfig apiConfig)
        {
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiConfig.GetBearerToken());
        }

        public async Task<StartApplicationResponse> StartApplication(StartApplicationRequest request)
        {
            return await HttpCall<StartApplicationResponse>(async () => await _httpClient.PostAsJsonAsync(new Uri("applications/start"), request));
        }

        public async Task<object> GetApplicationData(Guid applicationId)
        {
            return await HttpCall<object>(async () => await _httpClient.GetAsync(new Uri($"applications/{applicationId}/applicationData")));
        }

//        public async Task<Sequence> GetCurrentSequence(Guid applicationId)
//        {
//            return await _httpClient.GetAsync(new Uri($"applications/{applicationId}/sequences/current")).Result.Content.ReadAsAsync<Sequence>();
//        }
//
//        public async Task<List<Workflow>> GetWorkflows()
//        {
//            using (var response = await _httpClient.GetAsync(new Uri("workflows", UriKind.Relative)))
//            {
//                return await response.Content.ReadAsAsync<List<Workflow>>();
//            }
//        }
        
        private async Task<T> HttpCall<T>(Func<Task<HttpResponseMessage>> httpClientAction)
        {
            var httpResponse = await httpClientAction();

            if (httpResponse.IsSuccessStatusCode)
            {
                return await httpResponse.Content.ReadAsAsync<T>();
            }

            throw new HttpRequestException($"Error sending {httpResponse.RequestMessage.Method} to {httpResponse.RequestMessage.RequestUri}. Returned: {await httpResponse.Content.ReadAsStringAsync()}");
        }
    }
}