using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace ABCRetail.Services
{
    public class AzureFileStorageService
    {
        private readonly ShareClient _shareClient;

        public AzureFileStorageService(IConfiguration configuration)
        {

            // Retrieve the connection string from configuration
            string connectionString =
                configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException(
                    "AzureStorage connection string is not configured.");

            // Retrieve the share name from configuration
            string shareName =
                configuration["AzureFileStorage:ShareName"]
                ?? throw new InvalidOperationException(
                    "AzureFileStorage:ShareName is not configured.");

            _shareClient = new ShareClient(
                connectionString,
                shareName);

            _shareClient.CreateIfNotExists();
        }

        public async Task UploadFileAsync(
            IFormFile file,
            string directoryName = "documents")
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException(
                    "The file is empty or invalid.");
            }

            ShareDirectoryClient directoryClient =
                _shareClient.GetDirectoryClient(directoryName);

            await directoryClient.CreateIfNotExistsAsync();

            ShareFileClient fileClient =
                directoryClient.GetFileClient(file.FileName);

            using Stream stream = file.OpenReadStream();

            await fileClient.CreateAsync(file.Length);

            await fileClient.UploadRangeAsync(
                new HttpRange(0, file.Length),
                stream);
        }

        public async Task<List<string>> GetFilesAsync(
            string directoryName = "documents")
        {
            ShareDirectoryClient directoryClient =
                _shareClient.GetDirectoryClient(directoryName);

            await directoryClient.CreateIfNotExistsAsync();

            List<string> files = new();

            await foreach (
                ShareFileItem item in directoryClient.GetFilesAndDirectoriesAsync())
            {
                if (!item.IsDirectory)
                {
                    files.Add(item.Name);
                }
            }

            return files;
        }

        public async Task<Stream?> DownloadFileAsync(
            string fileName,
            string directoryName = "documents")
        {
            ShareDirectoryClient directoryClient =
                _shareClient.GetDirectoryClient(directoryName);

            ShareFileClient fileClient =
                directoryClient.GetFileClient(fileName);

            if (!await fileClient.ExistsAsync())
            {
                return null;
            }

            ShareFileDownloadInfo download =
                await fileClient.DownloadAsync();

            MemoryStream memoryStream = new();

            await download.Content.CopyToAsync(memoryStream);

            memoryStream.Position = 0;

            return memoryStream;
        }

        public async Task DeleteFileAsync(
            string fileName,
            string directoryName = "documents")
        {
            ShareDirectoryClient directoryClient =
                _shareClient.GetDirectoryClient(directoryName);

            ShareFileClient fileClient =
                directoryClient.GetFileClient(fileName);

            await fileClient.DeleteIfExistsAsync();
        }
    }
}
