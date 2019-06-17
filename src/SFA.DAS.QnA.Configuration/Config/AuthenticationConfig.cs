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
}