using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.Files.DeleteFile
{
    public class DeleteFileRequest : IRequest<HandlerResponse<bool>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }
        public string PageId { get; }
        public string QuestionId { get; }
        public string FileName { get; }

        public DeleteFileRequest(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
            PageId = pageId;
            QuestionId = questionId;
            FileName = fileName;
        }
    }
    
    public class DeleteFileHandler : IRequestHandler<DeleteFileRequest, HandlerResponse<bool>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IOptions<FileStorageConfig> _fileStorageConfig;

        public DeleteFileHandler(QnaDataContext dataContext, IOptions<FileStorageConfig> fileStorageConfig)
        {
            _dataContext = dataContext;
            _fileStorageConfig = fileStorageConfig;
        }
        
        public async Task<HandlerResponse<bool>> Handle(DeleteFileRequest request, CancellationToken cancellationToken)
        {
            
            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            
            if (section == null)
            {
                return new HandlerResponse<bool>(success:false, message:$"Section {request.SectionId} in Application {request.ApplicationId} does not exist.");
            }
            
            var qnaData = new QnAData(section.QnAData);
            var page = qnaData.Pages.FirstOrDefault(p => p.PageId == request.PageId);

            if (page == null)
            {
                return new HandlerResponse<bool>(success:false, message:$"Page {request.PageId} in Application {request.ApplicationId} does not exist.");
            }

            if (page.Questions.All(q => q.Input.Type != "FileUpload"))
            {
                return new HandlerResponse<bool>(success:false, message:$"Page {request.PageId} in Application {request.ApplicationId} does not contain any File Upload questions.");
            }
            
            if (page.PageOfAnswers == null || !page.PageOfAnswers.Any())
            {
                return new HandlerResponse<bool>(success:false, message:$"Page {request.PageId} in Application {request.ApplicationId} does not contain any uploads.");
            }

            var container = await ContainerHelpers.GetContainer(_fileStorageConfig.Value.StorageConnectionString, _fileStorageConfig.Value.ContainerName);
            var directory = ContainerHelpers.GetDirectory(request.ApplicationId, section.SequenceId, request.SectionId, request.PageId, request.QuestionId, container);

            var answer = page.PageOfAnswers.SingleOrDefault(poa => poa.Answers.Any(a => a.QuestionId == request.QuestionId && a.Value == request.FileName));
            if (answer is null)
            {
                return new HandlerResponse<bool>(success:false, message:$"Question {request.QuestionId} on Page {request.PageId} in Application {request.ApplicationId} does not contain an upload named {request.FileName}.");
            }

            page.PageOfAnswers.Remove(answer);
            section.QnAData = qnaData;
            await _dataContext.SaveChangesAsync(cancellationToken);

            var blobRef = directory.GetBlobReference(request.FileName);
            await blobRef.DeleteAsync(cancellationToken);
            
            return new HandlerResponse<bool>(true);
        }
    }
}