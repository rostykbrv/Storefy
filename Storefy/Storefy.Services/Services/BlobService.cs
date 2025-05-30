using Azure.Storage.Blobs.Models;
using Storefy.Interfaces.Services;

namespace Storefy.Services.Services;

/// <summary>
/// Provides service to interact with Blob Storage.
/// </summary>
/// <inheritdoc cref="IBlobService"/>
public class BlobService : IBlobService
{
    private readonly IAzureBlobService _blobServiceClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlobService"/> class.
    /// </summary>
    /// <param name="blobServiceClient">The Azure Blob Service client.</param>
    public BlobService(IAzureBlobService blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    /// <inheritdoc />
    public async Task<Stream> GetBlobAsync(string blobName, string containerName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        if (await blobClient.ExistsAsync())
        {
            var blobInfo = await blobClient.DownloadStreamingAsync();
            return blobInfo.Value.Content;
        }

        var noImageBlobClient = blobContainerClient.GetBlobClient("no-image");
        var noImageInfo = await noImageBlobClient.DownloadStreamingAsync();
        return noImageInfo.Value.Content;
    }

    /// <inheritdoc />
    public async Task UploadImageAsync(string blobName, string base64ImageContent, string containerName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var imageDataIndex = base64ImageContent.IndexOf(",", StringComparison.Ordinal) + 1;
        var imageData = base64ImageContent[imageDataIndex..];

        var byteArray = Convert.FromBase64String(imageData);
        using var stream = new MemoryStream(byteArray);

        var blobClient = blobContainerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = "image/jpeg" } });
    }
}
