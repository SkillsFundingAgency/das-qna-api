using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Schema;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.Files.DownloadFile
{
    public class DownloadFileHandler :IRequestHandler<DownloadFileRequest, HandlerResponse<DownloadFile>>
    {
        private readonly IOptions<FileStorageConfig> _fileStorageConfig;
        private readonly IEncryptionService _encryptionService;
        private readonly QnaDataContext _dataContext;

        public DownloadFileHandler(IOptions<FileStorageConfig> fileStorageConfig, IEncryptionService encryptionService, QnaDataContext dataContext)
        {
            _fileStorageConfig = fileStorageConfig;
            _encryptionService = encryptionService;
            _dataContext = dataContext;
        }
        
        public async Task<HandlerResponse<DownloadFile>> Handle(DownloadFileRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            
            if (section == null)
            {
                return new HandlerResponse<DownloadFile>(success:false, message:$"Section {request.SectionId} in Application {request.ApplicationId} does not exist.");
            }
            
            var page = section.QnAData.Pages.FirstOrDefault(p => p.PageId == request.PageId);

            if (page == null)
            {
                return new HandlerResponse<DownloadFile>(success:false, message:$"Page {request.PageId} in Application {request.ApplicationId} does not exist.");
            }

            if (page.Questions.All(q => q.Input.Type != "FileUpload"))
            {
                return new HandlerResponse<DownloadFile>(success:false, message:$"Page {request.PageId} in Application {request.ApplicationId} does not contain any File Upload questions.");
            }
            
            if (page.PageOfAnswers == null || !page.PageOfAnswers.Any())
            {
                return new HandlerResponse<DownloadFile>(success:false, message:$"Page {request.PageId} in Application {request.ApplicationId} does not contain any uploads.");
            }

            var container = await ContainerHelpers.GetContainer(_fileStorageConfig.Value.StorageConnectionString, _fileStorageConfig.Value.ContainerName);
            var directory = ContainerHelpers.GetDirectory(request.ApplicationId, section.SequenceId, request.SectionId, request.PageId, request.QuestionId, container);
            
            if (!(request.FileName is null)) return await SpecifiedFile(request, cancellationToken, page, directory);
            
            var blobs = directory.ListBlobs(useFlatBlobListing: true).ToList();
            if (blobs.Count() == 1)
            {
                var answer = page.PageOfAnswers.SelectMany(poa => poa.Answers).Single(a => a.QuestionId == request.QuestionId);
                return await IndividualFile(answer.Value, cancellationToken, directory);
            }

            if (!blobs.Any()) return new HandlerResponse<DownloadFile>(success: false, message: $"Page {request.PageId} in Application {request.ApplicationId} does not contain any uploads.");
                
            return await ZippedMultipleFiles(request, cancellationToken, page, directory);
        }

        private async Task<HandlerResponse<DownloadFile>> ZippedMultipleFiles(DownloadFileRequest request, CancellationToken cancellationToken, Page page, CloudBlobDirectory directory)
        {
            using (var zipStream = new MemoryStream())
            {
                await ZipUploadedFiles(request, cancellationToken, zipStream, page, directory);
                var newStream = new MemoryStream();
                zipStream.CopyTo(newStream);
                newStream.Position = 0;
                return new HandlerResponse<DownloadFile>(new DownloadFile() {ContentType = "application/zip", FileName = $"{request.QuestionId}_uploads.zip", Stream = newStream});
            }
        }

        private async Task ZipUploadedFiles(DownloadFileRequest request, CancellationToken cancellationToken, MemoryStream zipStream, Page page, CloudBlobDirectory directory)
        {
            using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var answer in page.PageOfAnswers.SelectMany(poa => poa.Answers).Where(a => a.QuestionId == request.QuestionId))
                {
                    var blobStream = await GetFileStream(cancellationToken, directory, answer.Value);

                    var zipEntry = zipArchive.CreateEntry(answer.Value);
                    using (var entryStream = zipEntry.Open())
                    {
                        blobStream.Item1.CopyTo(entryStream);
                    }
                }
            }

            zipStream.Position = 0;
        }

        private async Task<HandlerResponse<DownloadFile>> SpecifiedFile(DownloadFileRequest request, CancellationToken cancellationToken, Page page, CloudBlobDirectory directory)
        {
            var answer = page.PageOfAnswers.SelectMany(poa => poa.Answers).SingleOrDefault(a => a.Value == request.FileName && a.QuestionId == request.QuestionId);
            if (answer is null)
            {
                return new HandlerResponse<DownloadFile>(success: false, message: $"Question {request.QuestionId} on Page {request.PageId} in Application {request.ApplicationId} does not contain an upload named {request.FileName}");
            }

            return await IndividualFile(request.FileName, cancellationToken, directory);
        }

        private async Task<HandlerResponse<DownloadFile>> IndividualFile(string filename, CancellationToken cancellationToken, CloudBlobDirectory directory)
        {
            var blobStream = await GetFileStream(cancellationToken, directory, filename);

            return new HandlerResponse<DownloadFile>(new DownloadFile() {ContentType = blobStream.Item2, FileName = filename, Stream = blobStream.Item1});
        }

        private async Task<Tuple<Stream, string>> GetFileStream(CancellationToken cancellationToken, CloudBlobDirectory directory, string blobName)
        {
            var blobReference = directory.GetBlobReference(blobName);
            var blobStream = new MemoryStream();

            await blobReference.DownloadToStreamAsync(blobStream, null, new BlobRequestOptions() {DisableContentMD5Validation = true}, null, cancellationToken);
            blobStream.Position = 0;

            var decryptedStream = _encryptionService.Decrypt(blobStream);
            
            return new Tuple<Stream, string>(decryptedStream, blobReference.Properties.ContentType);
        }
    }
}