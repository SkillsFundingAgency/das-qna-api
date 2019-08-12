using System;
using System.Collections.Generic;
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

namespace SFA.DAS.QnA.Application.Commands.Files.UploadFile
{
    public class UploadFileHandler : IRequestHandler<UploadFileRequest, HandlerResponse<SetPageAnswersResponse>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IOptions<FileStorageConfig> _fileStorageConfig;
        private readonly IEncryptionService _encryptionService;

        public UploadFileHandler(QnaDataContext dataContext, IOptions<FileStorageConfig> fileStorageConfig, IEncryptionService encryptionService)
        {
            _dataContext = dataContext;
            _fileStorageConfig = fileStorageConfig;
            _encryptionService = encryptionService;
        }
        
        public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(UploadFileRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            
            var qnaData = new QnAData(section.QnAData);
            var page = qnaData.Pages.FirstOrDefault(p => p.PageId == request.PageId);

            if (page is null) return new HandlerResponse<SetPageAnswersResponse>(success: false, message: $"The page {request.PageId} in section {request.SectionId} does not exist.");
            
            if (page.AllowMultipleAnswers) return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for Multiple Answers pages.");

            var container = await ContainerHelpers.GetContainer(_fileStorageConfig.Value.StorageConnectionString, _fileStorageConfig.Value.ContainerName);

            foreach (var file in request.Files)
            {
                var questionFolder = ContainerHelpers.GetDirectory(request.ApplicationId, section.SequenceId, request.SectionId, request.PageId, request.QuestionId, container);
            
                var blob = questionFolder.GetBlockBlobReference(file.FileName);
                blob.Properties.ContentType = file.ContentType;

                var encryptedFileStream = _encryptionService.Encrypt(file.OpenReadStream());
                
                
                await blob.UploadFromStreamAsync(encryptedFileStream, cancellationToken);

                if (page.PageOfAnswers is null)
                {
                    page.PageOfAnswers = new List<PageOfAnswers>();
                }

                var foundExistingOnPage = page.PageOfAnswers.SelectMany(a => a.Answers).Any(answer => answer.QuestionId == file.Name && answer.Value == file.FileName);
                
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
        
        
    }
}


