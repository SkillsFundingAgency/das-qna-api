using System.Collections.Generic;

namespace SFA.DAS.QnA.Api.Types.Page
{
    public class Input
    {
        public string Type { get; set; }
        public string InputClasses { get; set; }
        public List<Option> Options { get; set; }
        public List<ValidationDefinition> Validations { get; set; }
        public string DataEndpoint { get; set; }
    }
}