using System.Collections.Generic;

namespace SFA.DAS.Qna.Api.Types.Page
{
    public class Option
    {
        public List<Question> FurtherQuestions { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
    }
}