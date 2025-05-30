using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Storefy.Interfaces.Services;

namespace Storefy.Services.Services;

/// <summary>
/// Provides service to interact with Azure Blob Storage.
/// </summary>
/// <inheritdoc cref="IAzureBlobService"/>
public class AzureBlobService : IAzureBlobService
{
    private readonly BlobServiceClient _blobServiceClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureBlobService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration from which
    /// to retrieve the Azure Blob Storage connection string.</param>
    public AzureBlobService(IConfiguration configuration)
    {
        _blobServiceClient = new BlobServiceClient(configuration
            .GetSection("AzureBlobStorage:ConnectionString").Value);
    }

    /// <inheritdoc />
    public BlobContainerClient GetBlobContainerClient(string containerName)
    {
        return _blobServiceClient.GetBlobContainerClient(containerName);
    }
}