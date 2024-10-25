﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;
using Microsoft.Extensions.Options;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Application.Commands.CreateSnapshot;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using SFA.DAS.QnA.Application.Commands.Files;
using System.Text;
using System.IO;
using Azure.Storage.Blobs.Models;
using System.Diagnostics;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.CreateSnapshotTests
{
    [TestFixture]
    public class CreateSnapshotTestBase
    {
        private const string _fileStorageConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1";
        private string _fileStorageContainerName = Guid.NewGuid().ToString();

        protected Guid ApplicationId;
        protected Guid SequenceId;
        protected Guid SectionId;
        protected string PageId;
        protected string QuestionId;
        protected string Filename;

        protected CreateSnapshotHandler Handler;

        protected QnaDataContext DataContext;
        protected BlobContainerClient ContainerClient;

        [SetUp]
        public async Task SetUp()
        {
            DataContext = DataContextHelpers.GetInMemoryDataContext();
            var fileStorageConfig = GetFileStorageConfig();
            var logger = Substitute.For<ILogger<CreateSnapshotHandler>>();

            Handler = new CreateSnapshotHandler(DataContext, fileStorageConfig, logger);

            ApplicationId = Guid.NewGuid();
            SequenceId = Guid.NewGuid();
            SectionId = Guid.NewGuid();
            PageId = "1";
            QuestionId = "Q1";
            Filename = "file.txt";

            ContainerClient = await  GetContainerClient(fileStorageConfig);

            await DataContext.Applications.AddAsync(new Data.Entities.Application() { Id = ApplicationId, ApplicationData = "{}" });

            await DataContext.ApplicationSequences.AddAsync(new ApplicationSequence()
            {
                ApplicationId = ApplicationId,
                Id = SequenceId
            });

            await DataContext.ApplicationSections.AddAsync(new ApplicationSection()
            {
                ApplicationId = ApplicationId,
                Id = SectionId,
                SequenceId = SequenceId,
                QnAData = new QnAData()
                {
                    Pages = new List<Page>
                    {
                        new Page()
                        {
                            SectionId = SectionId,
                            SequenceId = SequenceId,
                            PageId = PageId,
                            Questions = new List<Question>{new Question(){QuestionId = QuestionId, Input = new Input { Type = "FileUpload" } }},
                            PageOfAnswers = new List<PageOfAnswers>(){ new PageOfAnswers { Id = Guid.NewGuid(), Answers = new List<Answer> { new Answer { QuestionId = QuestionId, Value = Filename } } } },
                            Next = new List<Next>(),
                            Active = true
                        }
                    }
                }
            });

            await DataContext.SaveChangesAsync();

            await AddFile(ApplicationId, SequenceId, SectionId, PageId, QuestionId, Filename, ContainerClient);
        }

        [TearDown]
        public void TearDown()
        {
            if (ContainerClient != null)
            {
                ContainerClient.DeleteIfExists();
            }
        }

        private IOptions<FileStorageConfig> GetFileStorageConfig()
        {
            return Options.Create(new FileStorageConfig { ContainerName = _fileStorageContainerName, StorageConnectionString = _fileStorageConnectionString });
        }

        private static async Task<BlobContainerClient> GetContainerClient(IOptions<FileStorageConfig> config)
        {
            return await ContainerHelpers.GetContainer(config.Value.StorageConnectionString, config.Value.ContainerName);
        }

        private static async Task AddFile(Guid applicationId, Guid sequenceId, Guid sectionId, string pageId, string questionId, string filename, BlobContainerClient container)
        {
            var questionDirectory = ContainerHelpers.GetDirectoryPath(applicationId, sequenceId, sectionId, pageId, questionId);

            var fullBlobPath = $"{questionDirectory}/{filename}";



            Trace.WriteLine($"Writing blob ->  {fullBlobPath}");

            byte[] byteArray = Encoding.ASCII.GetBytes(filename);

            using (var stream = new MemoryStream(byteArray))
            {
                var fileBlob = container.GetBlobClient(fullBlobPath);
                await fileBlob.UploadAsync(stream, new BlobHttpHeaders { ContentType = "text/plain" });
            }
        }

        protected async Task<bool> FileExists(Guid applicationId, Guid sequenceId, Guid sectionId, string pageId, string questionId, string filename, BlobContainerClient container)
        {
            var questionDirectory = ContainerHelpers.GetDirectoryPath(applicationId, sequenceId, sectionId, pageId, questionId);

            var fileBlob = container.GetBlobClient($"{questionDirectory}/{filename}");

            return await fileBlob.ExistsAsync();

        }
    }
}
