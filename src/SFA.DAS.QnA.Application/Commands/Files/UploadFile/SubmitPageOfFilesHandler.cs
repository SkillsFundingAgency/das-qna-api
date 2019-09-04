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
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.Files.UploadFile
{
    public class SubmitPageOfFilesHandler : SetAnswersBase, IRequestHandler<SubmitPageOfFilesRequest, HandlerResponse<SetPageAnswersResponse>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IOptions<FileStorageConfig> _fileStorageConfig;
        private readonly IEncryptionService _encryptionService;
        private readonly IAnswerValidator _answerValidator;

        public SubmitPageOfFilesHandler(QnaDataContext dataContext, IOptions<FileStorageConfig> fileStorageConfig, IEncryptionService encryptionService, IAnswerValidator answerValidator)
        {
            _dataContext = dataContext;
            _fileStorageConfig = fileStorageConfig;
            _encryptionService = encryptionService;
            _answerValidator = answerValidator;
        }
        
        public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(SubmitPageOfFilesRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            
            var qnaData = new QnAData(section.QnAData);
            var page = qnaData.Pages.FirstOrDefault(p => p.PageId == request.PageId);

            if (page is null) return new HandlerResponse<SetPageAnswersResponse>(success: false, message: $"The page {request.PageId} in section {request.SectionId} does not exist.");
            
            if (page.AllowMultipleAnswers) return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for Multiple Answers pages.");

            if (page.Questions.Any(q => q.Input.Type != "FileUpload"))
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "Pages cannot contain a mixture of FileUploads and other Question Types.");
            }

            var answersToValidate = new List<Answer>();
            foreach (var file in request.Files)
            {
                var answer = new Answer() {QuestionId = file.Name, Value = file.FileName};
                answersToValidate.Add(answer);
            }
            
            var validationErrors = _answerValidator.Validate(answersToValidate, page);
            if (validationErrors.Any())
            {
                return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(validationErrors));
            }

            page.Complete = true;
            
            var container = await ContainerHelpers.GetContainer(_fileStorageConfig.Value.StorageConnectionString, _fileStorageConfig.Value.ContainerName);

            foreach (var file in request.Files)
            {
                var questionIdFromFileName = file.Name;
                var questionFolder = ContainerHelpers.GetDirectory(request.ApplicationId, section.SequenceId, request.SectionId, request.PageId, questionIdFromFileName, container);
            
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

            
            MarkFeedbackComplete(page);

            var nextAction = GetNextAction(page, answersToValidate, section, _dataContext);
            
            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));
        }
    }
}


