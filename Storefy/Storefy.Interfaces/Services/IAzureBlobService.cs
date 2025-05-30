using Azure.Storage.Blobs;

namespace Storefy.Interfaces.Services;

/// <summary>
/// Interface for the AzureBlob Service. Defines the service operations related to the AzureBlob.
/// </summary>
public interface IAzureBlobService
{
    /// <summary>
    /// Gets a BlobContainerClient which can be used to manipulate
    /// and navigate resources under a specific container in Blob Storage.
    /// </summary>
    /// <param name="containerName">The name of the Blob container.</param>
    /// <returns>A BlobContainerClient for the specified container.</returns>
    BlobContainerClient GetBlobContainerClient(string containerName);
}
