using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace SFA.DAS.QnA.Application.Commands.Files
{
    public static class ContainerHelpers
    {
        public static async Task<BlobContainerClient> GetContainer(string connectionString, string containerName)
        {
            var serviceClient = new BlobServiceClient(connectionString);
            var containerClient = serviceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();
            return containerClient;
        }

        public static string GetDirectoryPath(Guid applicationId, Guid sequenceId, Guid sectionId, string pageId, string questionId)
        {
            var applicationFolder = applicationId.ToString();
            var sequenceFolder = sequenceId.ToString();
            var sectionFolder = sectionId.ToString();
            var pageFolder = pageId.ToLower();

            return string.IsNullOrEmpty(questionId)
                ? $"{applicationFolder}/{sequenceFolder}/{sectionFolder}/{pageFolder}"
                : $"{applicationFolder}/{sequenceFolder}/{sectionFolder}/{pageFolder}/{questionId.ToLower()}";
        }
    }
}
