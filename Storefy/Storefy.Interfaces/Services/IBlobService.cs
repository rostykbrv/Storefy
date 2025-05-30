namespace Storefy.Interfaces.Services;

/// <summary>
/// Provides an interface for interacting with Blob storage.
/// </summary>
public interface IBlobService
{
    /// <summary>
    /// Asynchronously retrieves a blob from the specified container.
    /// </summary>
    /// <param name="blobName">The name of the blob to retrieve.</param>
    /// <param name="containerName">The name of the container in which the blob is located.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a Stream with the blob data.</returns>
    Task<Stream> GetBlobAsync(string blobName, string containerName);

    /// <summary>
    /// Asynchronously uploads an image to the specified container.
    /// </summary>
    /// <param name="blobName">The name to assign to the uploaded blob.</param>
    /// <param name="base64ImageContent">The content of the image to upload, in base64 format.</param>
    /// <param name="containerName">The name of the container to which to upload the image.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UploadImageAsync(string blobName, string base64ImageContent, string containerName);
}
