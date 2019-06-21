using System;

namespace SFA.DAS.Qna.Api.Types
{
    public class Sequence
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public Guid ApplicationId { get; set; }
        public int SequenceNumber { get; set; }
        public bool IsActive { get; set; }
        
        public bool NotRequired { get; set; }
    }
}