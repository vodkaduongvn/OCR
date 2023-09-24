using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System.Text;

namespace M3Tech.OCR.API.PredictionDocument.Services
{
    public interface IAzureStorageService
    {
        Task<byte[]> GetLatestDocumentAsync();
        Task<byte[]> GetDocumentByFileNameAsync(string fileName);
        Task UploadFileToAzureStorage(string fileName, string message);
    }
    public class AzureStorageService:IAzureStorageService
    {
        private readonly  BlobContainerClient _containerClient;
        public AzureStorageService(
            string azureStorageConnectionString
            , string containerClientName) 
        {
            BlobServiceClient blobServiceClient = new(azureStorageConnectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerClientName);
        }

        public async Task<byte[]> GetDocumentByFileNameAsync(string fileName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                using var ms = new MemoryStream();
                await blobClient.DownloadToAsync(ms);

                return ms.ToArray();
            }
            return Array.Empty<byte>();
        }

        public async Task<byte[]> GetLatestDocumentAsync()
        {
            var blobs = _containerClient.GetBlobs();
            BlobItem latestBlob = blobs.OrderByDescending(x => x.Properties.CreatedOn).FirstOrDefault();

            if (latestBlob != null)
            {
                BlobClient blobClient = _containerClient.GetBlobClient(latestBlob.Name);

                if (await blobClient.ExistsAsync())
                {
                    using var ms = new MemoryStream();
                    await blobClient.DownloadToAsync(ms);

                    return ms.ToArray();
                }
            }

            return Array.Empty<byte>();
        }

        public async Task UploadFileToAzureStorage(string fileName, string message)
        {
            try
            {
                var appendBlobClient = _containerClient.GetAppendBlobClient(fileName);
                if (!appendBlobClient.Exists())
                    appendBlobClient.CreateIfNotExists();

                //Saving message
                var content = Encoding.UTF8.GetBytes(message += Environment.NewLine);

                using (var ms = new MemoryStream(content))
                {
                    await appendBlobClient.AppendBlockAsync(ms);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while an update to azure server", e);
            }
        }
    }
}
