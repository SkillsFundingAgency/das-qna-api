using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.Files.DownloadFile
{
    internal class DownloadFileService
    {
        private readonly IOptions<FileStorageConfig> _fileStorageConfig;
        private readonly IEncryptionService _encryptionService;

        internal DownloadFileService(IOptions<FileStorageConfig> fileStorageConfig, IEncryptionService encryptionService)
        {
            _fileStorageConfig = fileStorageConfig;
            _encryptionService = encryptionService;
        }

        internal async Task<HandlerResponse<DownloadFile>> GetDownloadFile(Guid applicationId, ApplicationSection section, string pageId, string questionId, string fileName, CancellationToken cancellationToken)
        {
            var page = section.QnAData.Pages.FirstOrDefault(p => p.PageId == pageId);

            if (page == null)
            {
                return new HandlerResponse<DownloadFile>(success: false, message: $"Page {pageId} in Application {applicationId} does not exist.");
            }

            if (page.Questions.All(q => q.Input.Type != "FileUpload"))
            {
                return new HandlerResponse<DownloadFile>(success: false, message: $"Page {pageId} in Application {applicationId} does not contain any File Upload questions.");
            }

            if (page.PageOfAnswers == null || !page.PageOfAnswers.Any())
            {
                return new HandlerResponse<DownloadFile>(success: false, message: $"Page {pageId} in Application {applicationId} does not contain any uploads.");
            }

            var container = await ContainerHelpers.GetContainer(_fileStorageConfig.Value.StorageConnectionString, _fileStorageConfig.Value.ContainerName);
            var questionDirectory = ContainerHelpers.GetDirectory(applicationId, section.SequenceId, section.Id, pageId, questionId);
            var pageDirectory = ContainerHelpers.GetDirectory(applicationId, section.SequenceId, section.Id, pageId, null);

            if ((questionId is null)) return await PageFiles(applicationId, pageId, cancellationToken, page, container);

            if (!(fileName is null)) return await SpecifiedFile(applicationId, fileName, pageId, questionId, cancellationToken, page, container);

            var blobs = container.GetBlobs(traits: BlobTraits.None, states: BlobStates.None, prefix: $"{questionDirectory}/", cancellationToken).ToList();
            if (blobs.Count() == 1)
            {
                var answer = page.PageOfAnswers.SelectMany(poa => poa.Answers).Single(a => a.QuestionId == questionId);
                return await IndividualFile(answer.Value, cancellationToken, container);
            }

            if (!blobs.Any()) return new HandlerResponse<DownloadFile>(success: false, message: $"Page {pageId} in Application {applicationId} does not contain any uploads.");

            return await ZippedMultipleFiles(questionId, cancellationToken, page, container);
        }

        private async Task<HandlerResponse<DownloadFile>> PageFiles(Guid applicationId, string pageId, CancellationToken cancellationToken, Page page, BlobContainerClient container)
        {
            // Get All answers on the page./
            // Loop them.
            // for each question...
            //     get the blobdirectory for that question
            //     download any files from that directory
            //     add to the zip
            using (var zipStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    foreach (var answer in page.PageOfAnswers.SelectMany(poa => poa.Answers))
                    {
                        var blobName = $"{answer.QuestionId.ToLower()}/{answer.Value}";
                        var blobStream = await GetFileStream(cancellationToken, container, blobName);

                        var zipEntry = zipArchive.CreateEntry(answer.Value);
                        using (var entryStream = zipEntry.Open())
                        {
                            blobStream.Item1.CopyTo(entryStream);
                        }
                    }
                }

                zipStream.Position = 0;
                var newStream = new MemoryStream();
                zipStream.CopyTo(newStream);
                newStream.Position = 0;
                return new HandlerResponse<DownloadFile>(new DownloadFile() { ContentType = "application/zip", FileName = $"{applicationId}_{pageId}_uploads.zip", Stream = newStream });
            }
        }

        private async Task<HandlerResponse<DownloadFile>> ZippedMultipleFiles(string questionId, CancellationToken cancellationToken, Page page, BlobContainerClient container)
        {
            using (var zipStream = new MemoryStream())
            {
                await ZipUploadedFiles(questionId, cancellationToken, zipStream, page, container);
                zipStream.Position = 0;
                var newStream = new MemoryStream();
                zipStream.CopyTo(newStream);
                newStream.Position = 0;
                return new HandlerResponse<DownloadFile>(new DownloadFile() { ContentType = "application/zip", FileName = $"{questionId}_uploads.zip", Stream = newStream });
            }
        }

        private async Task ZipUploadedFiles(string questionId, CancellationToken cancellationToken, MemoryStream zipStream, Page page, BlobContainerClient container)
        {
            using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var answer in page.PageOfAnswers.SelectMany(poa => poa.Answers).Where(a => a.QuestionId == questionId))
                {
                    var blobStream = await GetFileStream(cancellationToken, container, answer.Value);

                    var zipEntry = zipArchive.CreateEntry(answer.Value);
                    using (var entryStream = zipEntry.Open())
                    {
                        blobStream.Item1.CopyTo(entryStream);
                    }
                }
            }
        }

        private async Task<HandlerResponse<DownloadFile>> SpecifiedFile(Guid applicationId, string fileName, string pageId, string questionId, CancellationToken cancellationToken, Page page, BlobContainerClient container)
        {
            var answer = page.PageOfAnswers.SelectMany(poa => poa.Answers).SingleOrDefault(a => a.Value == fileName && a.QuestionId == questionId);
            if (answer is null)
            {
                return new HandlerResponse<DownloadFile>(success: false, message: $"Question {questionId} on Page {pageId} in Application {applicationId} does not contain an upload named {fileName}");
            }

            return await IndividualFile(fileName, cancellationToken, container);
        }

        private async Task<HandlerResponse<DownloadFile>> IndividualFile(string filename, CancellationToken cancellationToken, BlobContainerClient container)
        {
            var blobStream = await GetFileStream(cancellationToken, container, filename);

            return new HandlerResponse<DownloadFile>(new DownloadFile() { ContentType = blobStream.Item2, FileName = filename, Stream = blobStream.Item1 });
        }

        private async Task<Tuple<Stream, string>> GetFileStream(CancellationToken cancellationToken, BlobContainerClient container, string blobName)
        {
            var blobReference = container.GetBlobClient(blobName);
            var blobStream = new MemoryStream();

            await blobReference.DownloadToAsync(blobStream, cancellationToken);
            blobStream.Position = 0;

            var decryptedStream = _encryptionService.Decrypt(blobStream);

            return new Tuple<Stream, string>(decryptedStream, blobReference.GetProperties().Value.ContentType);
        }
    }
}
