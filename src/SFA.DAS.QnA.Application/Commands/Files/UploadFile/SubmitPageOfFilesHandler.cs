using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.Files.UploadFile
{
    public class SubmitPageOfFilesHandler : SetAnswersBase, IRequestHandler<SubmitPageOfFilesRequest, HandlerResponse<SetPageAnswersResponse>>
    {
        private readonly IOptions<FileStorageConfig> _fileStorageConfig;
        private readonly IEncryptionService _encryptionService;
        private readonly IAnswerValidator _answerValidator;
        private readonly IFileContentValidator _fileContentValidator;
        private readonly ITagProcessingService _tagProcessingService;
        public SubmitPageOfFilesHandler(QnaDataContext dataContext, IOptions<FileStorageConfig> fileStorageConfig, IEncryptionService encryptionService, IAnswerValidator answerValidator, IFileContentValidator fileContentValidator, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService) : base(dataContext, notRequiredProcessor, tagProcessingService)
        {
            _fileStorageConfig = fileStorageConfig;
            _encryptionService = encryptionService;
            _answerValidator = answerValidator;
            _fileContentValidator = fileContentValidator;
            _tagProcessingService = tagProcessingService;
        }
        
        public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(SubmitPageOfFilesRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            
            var qnaData = new QnAData(section.QnAData);
            var page = qnaData.Pages.FirstOrDefault(p => p.PageId == request.PageId);

            if (page is null)
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: $"The page {request.PageId} in section {request.SectionId} does not exist.");
            }
            else if (page.AllowMultipleAnswers)
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for Multiple Answers pages.");
            }
            else if (page.Questions.Count > 0)
            {
                if (page.Questions.Any(q => !"FileUpload".Equals(q.Input?.Type, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "Pages cannot contain a mixture of FileUploads and other Question Types.");
                }
            }

            var answersToValidate = new List<Answer>();
            foreach (var file in request.Files)
            {
                var answer = new Answer() {QuestionId = file.Name, Value = file.FileName};
                answersToValidate.Add(answer);
            }
            
            // Need to add to answersToValidate here any existing answers on the page for questions not already in answersToValidate
            if (page.PageOfAnswers != null)
            {
                foreach (var pageOfAnswers in page.PageOfAnswers)
                {
                    foreach (var existingAnswer in pageOfAnswers.Answers)
                    {
                        if (answersToValidate.All(a => a.QuestionId != existingAnswer.QuestionId))
                        {
                            answersToValidate.Add(existingAnswer);
                        }
                    }
                }   
            }

            var validationErrors = _answerValidator.Validate(answersToValidate, page);
            if (validationErrors.Any())
            {
                return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(validationErrors));
            }

            var fileContentValidationErrors = _fileContentValidator.Validate(request.Files);
            if (fileContentValidationErrors.Any())
            {
                return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(fileContentValidationErrors));
            }

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

            MarkPageAsComplete(page);
            MarkPageFeedbackAsComplete(page);

            section.QnAData = qnaData;
            await _dataContext.SaveChangesAsync(cancellationToken);


            await UpdateApplicationData(request.ApplicationId, page, page.PageOfAnswers.SelectMany(poa => poa.Answers).ToList());
            
            var nextAction = GetNextActionForPage(section.Id, page.PageId);
            
            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));
        }
        
        private async Task UpdateApplicationData(Guid applicationId, Page page, List<Answer> answers)
        {
            var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == applicationId);
            var applicationData = JObject.Parse(application.ApplicationData);
            foreach (var question in page.Questions)
            {
                SetApplicationDataField(answers, applicationData, question);
            }

            application.ApplicationData = applicationData.ToString(Formatting.None);

            await _dataContext.SaveChangesAsync();
        }
        
        private static void SetApplicationDataField(List<Answer> answers, JObject applicationData, Question question)
        {
            if (string.IsNullOrWhiteSpace(question.QuestionTag)) return;

            if (applicationData.ContainsKey(question.QuestionTag))
            {
                applicationData[question.QuestionTag] = answers.Single(a => a.QuestionId == question.QuestionId).Value;
            }
            else
            {
                applicationData.Add(question.QuestionTag, new JValue(answers.Single(a => a.QuestionId == question.QuestionId).Value));
            }
        }
    }
}


