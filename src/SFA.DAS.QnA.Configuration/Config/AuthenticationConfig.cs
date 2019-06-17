using System;

namespace SFA.DAS.QnA.Configuration.Config
{
    public class AuthenticationConfig
    {
        public string Instance { get; set; }
        public Guid TenantId { get; set; }
        public Guid ClientId { get; set; }
        public string Audience { get; set; }
    }
    
//    "ApiAuthentication":{
//    "Instance": "https://login.microsoftonline.com/",
//    "TenantId": "45009c1d-fca9-4573-bd77-7bbf46a85d11",
//    "ClientId": "9c16bbb6-2d60-4e09-b723-9cdb85523175",
//    "Audience": "https://davegougegmail.onmicrosoft.com/SFA.DAS.AssessorService.Api"
//},
}