using System;
using MediatR;
using Newtonsoft.Json.Schema;
using SFA.DAS.Qna.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Commands.PageFeedback.UpsertFeedback
{
    public class UpsertFeedbackRequest : IRequest<HandlerResponse<Page>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }
        public string PageId { get; }
        public Feedback Feedback { get; }

        public UpsertFeedbackRequest(Guid applicationId, Guid sectionId, string pageId, Feedback feedback)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
            PageId = pageId;
            Feedback = feedback;
        }
    }
}