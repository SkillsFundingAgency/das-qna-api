using System;
using Microsoft.Identity.Client;

namespace SFA.DAS.QnA.Api.Client
{
    public class QnaApiConfig
    {
        public string GetBearerToken()
        {
            if (DisableBearerHeader)
            {
                return "";
            }

            var authority = $"https://login.microsoftonline.com/{TenantId}";

            var app = ConfidentialClientApplicationBuilder.Create(ClientId)
                .WithClientSecret(ClientSecret)
                .WithAuthority(new Uri(authority))
                .Build();

            var scopes = new[] { $"{ResourceId}/.default" };

            var result = app.AcquireTokenForClient(scopes).ExecuteAsync().Result;

            return result.AccessToken;
        }

        public string ResourceId { get; set; }

        public string ClientSecret { get; set; }

        public string ClientId { get; set; }

        public string TenantId { get; set; }

        public Uri BaseUri { get; set; }

        public bool DisableBearerHeader { get; set; }
    }
}