using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace SFA.DAS.QnA.Application.Commands.Files
{
    public static class ContainerHelpers
    {
        public static async Task<BlobContainerClient> GetContainer(string connectionString, string containerName)
        {
            var client = new BlobServiceClient(connectionString);
            var container = client.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();
            return container;
        }

        public static string GetDirectory(Guid applicationId, Guid sequenceId, Guid sectionId, string pageId, string questionId)
        {
            var path = $"{applicationId}/{sequenceId}/{sectionId}/{pageId.ToLower()}";

            if (questionId is null)
            {
                return path;
            }

            return $"{path}/{questionId.ToLower()}";
        }
    }
}