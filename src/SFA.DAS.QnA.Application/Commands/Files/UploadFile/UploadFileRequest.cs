using System;
using MediatR;
using Microsoft.AspNetCore.Http;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.Files.UploadFile
{
    public class UploadFileRequest : IRequest<HandlerResponse<SetPageAnswersResponse>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }
        public string PageId { get; }
        public string QuestionId { get; }
        public IFormFileCollection Files { get; }

        public UploadFileRequest(Guid applicationId, Guid sectionId, string pageId, string questionId, IFormFileCollection files)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
            PageId = pageId;
            QuestionId = questionId;
            Files = files;
        }
    }
}