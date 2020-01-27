using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.CreateSnapshot
{
    public class CreateSnapshotHandler : IRequestHandler<CreateSnapshotRequest, HandlerResponse<CreateSnapshotResponse>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IOptions<FileStorageConfig> _fileStorageConfig;
        private readonly ILogger<CreateSnapshotHandler> _logger;

        public CreateSnapshotHandler(QnaDataContext dataContext, IOptions<FileStorageConfig> fileStorageConfig, ILogger<CreateSnapshotHandler> logger)
        {
            _dataContext = dataContext;
            _fileStorageConfig = fileStorageConfig;
            _logger = logger;
        }

        public async Task<HandlerResponse<CreateSnapshotResponse>> Handle(CreateSnapshotRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.AsNoTracking().FirstOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
            if (application is null) return new HandlerResponse<CreateSnapshotResponse>(false, "Application does not exist");

            // Step 1a create application
            var newapplication = await CreateNewApplication(application, cancellationToken);

            // Step 1b create sequences
            await CopySequences(application, newapplication, cancellationToken);

            // Step 1c create sections
            await CopySections(application, newapplication, cancellationToken);

            // Step 2 copy over files
            await CopyFiles(application, newapplication, cancellationToken);

            return new HandlerResponse<CreateSnapshotResponse>(new CreateSnapshotResponse { ApplicationId = newapplication.Id });
        }

        private async Task<Data.Entities.Application> CreateNewApplication(Data.Entities.Application application, CancellationToken cancellationToken)
        {
            var newApplication = new Data.Entities.Application
            {
                ApplicationStatus = application.ApplicationStatus,
                WorkflowId = application.WorkflowId,
                Reference = application.Reference,
                CreatedAt = SystemTime.UtcNow(),
                ApplicationData = application.ApplicationData
            };

            _dataContext.Applications.Add(newApplication);
            await _dataContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Created Application entity: {newApplication.Id}");

            return newApplication;
        }

        private async Task CopySequences(Data.Entities.Application currentApplication, Data.Entities.Application newApplication, CancellationToken cancellationToken)
        {
            var sequences = await _dataContext.ApplicationSequences.AsNoTracking()
                .Where(seq => seq.ApplicationId == currentApplication.Id).ToListAsync(cancellationToken);

            var newApplicationSequences = sequences.Select(seq => new ApplicationSequence
            {
                ApplicationId = newApplication.Id,
                SequenceNo = seq.SequenceNo,
                IsActive = seq.IsActive
            }).ToList();

            await _dataContext.ApplicationSequences.AddRangeAsync(newApplicationSequences, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Created ApplicationSequence entities for Application: {newApplication.Id}");
        }

        private async Task CopySections(Data.Entities.Application currentApplication, Data.Entities.Application newApplication, CancellationToken cancellationToken)
        {
            var sections = await _dataContext.ApplicationSections.AsNoTracking()
                .Where(sec => sec.ApplicationId == currentApplication.Id).ToListAsync(cancellationToken: cancellationToken);

            var newApplicationSequences = await _dataContext.ApplicationSequences.AsNoTracking()
                .Where(seq => seq.ApplicationId == newApplication.Id).ToListAsync(cancellationToken);

            var newApplicationSections = new List<ApplicationSection>();

            foreach (var sequence in newApplicationSequences)
            {
                // copy over all sections into the new Application Sequence
                foreach (var section in sections.Where(sec => sec.SequenceNo == sequence.SequenceNo))
                {
                    var newSection = new ApplicationSection
                    {
                        Id = Guid.NewGuid(),
                        SequenceId = sequence.Id,
                        Title = section.Title,
                        LinkTitle = section.LinkTitle,
                        ApplicationId = newApplication.Id,
                        DisplayType = section.DisplayType,
                        QnAData = section.QnAData,
                        SectionNo = section.SectionNo,
                        SequenceNo = section.SequenceNo
                    };

                    // Adjust page info appropriately
                    foreach (var page in newSection.QnAData.Pages)
                    {
                        page.SectionId = newSection.Id;
                        page.SequenceId = newSection.SequenceId;
                    }

                    newApplicationSections.Add(newSection);
                }
            }

            await _dataContext.ApplicationSections.AddRangeAsync(newApplicationSections, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Created ApplicationSection entities for Application: {newApplication.Id}");
        }

        private async Task CopyFiles(Data.Entities.Application currentApplication, Data.Entities.Application newApplication, CancellationToken cancellationToken)
        {
            var sections = await _dataContext.ApplicationSections.AsNoTracking().Where(sec => sec.ApplicationId == currentApplication.Id).ToListAsync();
            var newSections = await _dataContext.ApplicationSections.AsNoTracking().Where(sec => sec.ApplicationId == newApplication.Id).ToListAsync();

            // go through each section in the application
            foreach (var section in sections)
            {
                var newSection = newSections.FirstOrDefault(s => s.SectionNo == section.SectionNo && s.SequenceNo == section.SequenceNo);
                if (newSection is null) continue;

                // go through each page within the section that has a FileUpload question
                foreach (var page in section.QnAData.Pages)
                {
                    if (page.Questions.Any(q => "FileUpload".Equals(q.Input?.Type)))
                    {
                        foreach (var pageOfAnswer in page.PageOfAnswers)
                        {
                            // for each answer see if it exists in Azure Storage and copy it across
                            foreach (var answer in pageOfAnswer.Answers)
                            {
                                if (!string.IsNullOrWhiteSpace(answer.Value))
                                {
                                    var originalFileUrl = $"{section.ApplicationId.ToString().ToLower()}/{section.SequenceId.ToString().ToLower()}/{section.Id.ToString().ToLower()}/{page.PageId.ToLower()}/{answer.QuestionId.ToLower()}/{answer.Value}";
                                    var snapshotFileUrl = $"{newSection.ApplicationId.ToString().ToLower()}/{newSection.SequenceId.ToString().ToLower()}/{newSection.Id.ToString().ToLower()}/{page.PageId.ToLower()}/{answer.QuestionId.ToLower()}/{answer.Value}";

                                    try
                                    {
                                        // get original file...
                                        var account = CloudStorageAccount.Parse(_fileStorageConfig.Value.StorageConnectionString);
                                        var client = account.CreateCloudBlobClient();
                                        var container = client.GetContainerReference(_fileStorageConfig.Value.ContainerName);

                                        var currentFileBlobReference = container.GetBlockBlobReference(originalFileUrl);

                                        // it exists so copy it into the new application
                                        if (currentFileBlobReference.Exists())
                                        {
                                            var snapshotFileBlobReference = container.GetBlockBlobReference(snapshotFileUrl);
                                            await snapshotFileBlobReference.StartCopyAsync(currentFileBlobReference);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError($"Error copying file in snapshot: {snapshotFileUrl} || Message: {ex.Message} || Stack trace: {ex.StackTrace}");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            _logger.LogInformation($"Copied over file uploads for Application: {newApplication.Id}");
        }
    }
}
