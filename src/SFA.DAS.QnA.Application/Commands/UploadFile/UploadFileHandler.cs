using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.UploadFile
{
    public class UploadFileHandler : IRequestHandler<UploadFileRequest, HandlerResponse<SetPageAnswersResponse>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IOptions<FileStorageConfig> _fileStorageConfig;

        public UploadFileHandler(QnaDataContext dataContext, IOptions<FileStorageConfig> fileStorageConfig)
        {
            _dataContext = dataContext;
            _fileStorageConfig = fileStorageConfig;
        }
        
        public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(UploadFileRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            
            var qnaData = new QnAData(section.QnAData);
            var page = qnaData.Pages.FirstOrDefault(p => p.PageId == request.PageId);

            if (page is null) return new HandlerResponse<SetPageAnswersResponse>(success: false, message: $"The page {request.PageId} in section {request.SectionId} does not exist.");
            
            if (page.AllowMultipleAnswers) return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for Multiple Answers pages.");

            var container = await GetContainer();

            foreach (var file in request.Files)
            {
                var questionFolder = GetDirectory(request.ApplicationId, section.SequenceId, request.SectionId, request.PageId, request.QuestionId, container);
            
                var blob = questionFolder.GetBlockBlobReference(file.FileName);
                blob.Properties.ContentType = file.ContentType;
                await blob.UploadFromStreamAsync(file.OpenReadStream(), cancellationToken);

                if (page.PageOfAnswers is null)
                {
                    page.PageOfAnswers = new List<PageOfAnswers>();
                }

                var foundExistingOnPage = page.PageOfAnswers.SelectMany(a => a.Answers).Any(answer => answer.QuestionId == file.Name);
                
                if (!foundExistingOnPage)
                {
                    page.PageOfAnswers.Add(new PageOfAnswers(){Id = Guid.NewGuid(), Answers = new List<Answer>()
                    {
                        new Answer()
                        {
                            QuestionId = file.Name,
                            Value = file.FileName
                        }
                    }});
                }

            }

            section.QnAData = qnaData;
            await _dataContext.SaveChangesAsync(cancellationToken);

            var nextAction = page.Next.First();
            
            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));
        }
        
        private async Task<CloudBlobContainer> GetContainer()
        {
            var account = CloudStorageAccount.Parse(_fileStorageConfig.Value.StorageConnectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference(_fileStorageConfig.Value.ContainerName);
            await container.CreateIfNotExistsAsync();
            return container;
        }
        
        private static CloudBlobDirectory GetDirectory(Guid applicationId, Guid sequenceId, Guid sectionId, string pageId, string questionId, CloudBlobContainer container)
        {
            var applicationFolder = container.GetDirectoryReference(applicationId.ToString());
            var sequenceFolder = applicationFolder.GetDirectoryReference(sequenceId.ToString());
            var sectionFolder = sequenceFolder.GetDirectoryReference(sectionId.ToString());
            var pageFolder = sectionFolder.GetDirectoryReference(pageId.ToLower());
            var questionFolder = pageFolder.GetDirectoryReference(questionId.ToLower());
            return questionFolder;
        }
    }
}