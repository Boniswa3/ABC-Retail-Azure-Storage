using Azure.Storage.Blobs;

namespace ABCRetail.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(IConfiguration configuration)
        {
            // Retrieve the connection string from configuration
            string connectionString = configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException("Azure Storage connection string not found.");

            // Initialize the BlobServiceClient
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Get a reference to the container (e.g., "productimages")
            _container = blobServiceClient.GetBlobContainerClient("productimages");

            _container.CreateIfNotExists();
        }
        // Upload an image to Blob Storage and return its URL
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            // Validate the file
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file selected.");
            }

            // Create a unique file name
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            // Get a reference to the blob
            BlobClient blobClient = _container.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                // Upload the file to Blob Storage
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobClient.Uri.ToString();
        }
    }
}
