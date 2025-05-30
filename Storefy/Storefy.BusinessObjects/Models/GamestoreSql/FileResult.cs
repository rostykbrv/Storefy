namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Result object for a file operation related to all files.
/// </summary>
public class FileResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileResult"/> class.
    /// </summary>
    /// <param name="fileBytes">The byte array of the file.</param>
    /// <param name="contentType">The content type of the file.</param>
    /// <param name="fileName">The name of the file.</param>
    public FileResult(byte[] fileBytes, string contentType, string fileName)
    {
        FileBytes = fileBytes;
        ContentType = contentType;
        FileName = fileName;
    }

    /// <summary>
    /// Gets the FileBytes property. Represents the byte array of the file.
    /// </summary>
    public byte[] FileBytes { get; }

    /// <summary>
    /// Gets the ContentType property. Represents the content type of the file.
    /// </summary>
    public string ContentType { get; }

    /// <summary>
    /// Gets the FileName property. Represents the name of the file.
    /// </summary>
    public string FileName { get; }
}
