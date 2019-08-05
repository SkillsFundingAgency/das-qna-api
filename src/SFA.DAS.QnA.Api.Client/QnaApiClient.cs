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

        public async Task<HttpResponseMessage> StartApplication(StartApplicationRequest request)
        {
            return await _httpClient.PostAsJsonAsync(new Uri("applications/start"), request);
        }

        public async Task<List<Workflow>> GetWorkflows()
        {
            using (var response = await _httpClient.GetAsync(new Uri("workflows", UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<List<Workflow>>();
            }
        }
    }
}