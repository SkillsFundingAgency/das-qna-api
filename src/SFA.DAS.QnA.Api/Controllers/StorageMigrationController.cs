using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.QnA.Application.Commands.Files;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Api.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StorageMigrationController : Controller
    {
        private QnaDataContext _dataContext;
        private IOptions<FileStorageConfig> _fileStorageConfig;
        private IEncryptionService _encryptionService;
        private readonly ILogger<StorageMigrationController> _log;

        public StorageMigrationController(QnaDataContext dataContext, IOptions<FileStorageConfig> fileStorageConfig, IEncryptionService encryptionService, ILogger<StorageMigrationController> log)
        {
            _dataContext = dataContext;
            _fileStorageConfig = fileStorageConfig;
            _encryptionService = encryptionService;
            _log = log;
        }

        [HttpPost("/storageMigration")]
        public async Task<ActionResult<FileMigrationResult>> Migrate()
        {
            var result = new FileMigrationResult {MigratedFiles = new List<MigratedFile>()};

            try
            {
                // get all sections where SectionNo = 3
                var sections = await _dataContext.ApplicationSections.Where(sec => (sec.SectionNo == 3 && sec.SequenceNo == 1) || sec.SequenceNo == 2).ToListAsync();

                foreach (var section in sections)
                {
                    var sectionId = section.Id;


                    foreach (var page in section.QnAData.Pages)
                    {
                        if (page.Questions.Any(q => q.Input.Type == "FileUpload"))
                        {
                            var sequenceId = page.SequenceId.Value;
                            foreach (var pageOfAnswer in page.PageOfAnswers)
                            {
                                foreach (var answer in pageOfAnswer.Answers)
                                {
                                    if (!string.IsNullOrWhiteSpace(answer.Value))
                                    {
                                        // get original file...
                                        var blobServiceClient = new BlobServiceClient(_fileStorageConfig.Value.StorageConnectionString);
                                        var containerClient = blobServiceClient.GetBlobContainerClient(_fileStorageConfig.Value.ContainerName);

                                        var applicationFolder = $"{section.ApplicationId.ToString().ToLower()}";
                                        var sequenceFolder = $"{sequenceId.ToString().ToLower()}";
                                        var sectionFolder = $"{sectionId.ToString().ToLower()}";
                                        var pageFolder = $"{page.PageId.ToLower()}";
                                        var questionFolder = $"{answer.QuestionId.ToLower()}";

                                        var originalBlobPath = $"{applicationFolder}/{sequenceFolder}/{sectionFolder}/{pageFolder}/{questionFolder}/{answer.Value}";
                                        var blobClient = containerClient.GetBlobClient(originalBlobPath);

                                        if (await blobClient.ExistsAsync())
                                        {
                                            var newFileUrl = $"{applicationFolder}/{sequenceFolder}/{sectionFolder}/{page.PageId}/{answer.QuestionId.ToLower()}/{answer.Value}";
                                            var newBlobClient = containerClient.GetBlobClient(newFileUrl);

                                            await newBlobClient.StartCopyFromUriAsync(blobClient.Uri);

                                            result.MigratedFiles.Add(new MigratedFile { From = blobClient.Name, To = newFileUrl });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation($"Error running file migration: {ex.Message}. Stack trace: {ex.StackTrace}");
                return new FileMigrationResult() {Error = ex.Message, ErrorStackTrace = ex.StackTrace};
            }

            return result;
        }
    }

    public class FileMigrationResult
    {
        public List<MigratedFile> MigratedFiles { get; set; }
        public string Error { get; set; }
        public string ErrorStackTrace { get; set; }
    }

    public class MigratedFile
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}