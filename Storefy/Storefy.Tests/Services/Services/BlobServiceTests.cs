using System.Text;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Storefy.Interfaces.Services;
using Storefy.Services.Services;

namespace Storefy.Tests.Services.Services;
public class BlobServiceTests
{
    private readonly Mock<IAzureBlobService> _blobServiceClientMock;
    private readonly BlobService _blobService;

    public BlobServiceTests()
    {
        _blobServiceClientMock = new Mock<IAzureBlobService>();
        _blobService = new BlobService(_blobServiceClientMock.Object);
    }

    [Fact]
    public async Task GetBlobAsync_ShouldReturnStream_WhenBlobExists()
    {
        // Arrange
        var containerName = "testcontainer";
        var blobName = "testblob";
        var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
        var blobClientMock = new Mock<BlobClient>();
        var noImageBlobClientMock = new Mock<BlobClient>();
        var blobContainerClientMock = new Mock<BlobContainerClient>();
        var responseMock = new Mock<Response<bool>>();

        responseMock.SetupGet(r => r.Value).Returns(true);
        blobClientMock.Setup(m => m.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(responseMock.Object);

        var blobDownloadInfo = BlobsModelFactory.BlobDownloadStreamingResult(content: expectedStream);
        var downloadResponse = Response.FromValue(blobDownloadInfo, new Mock<Response>().Object);
        blobClientMock.Setup(x => x.DownloadStreamingAsync(default, default)).ReturnsAsync(downloadResponse);

        blobContainerClientMock.Setup(x => x.GetBlobClient(blobName)).Returns(blobClientMock.Object);
        blobContainerClientMock.Setup(x => x.GetBlobClient("no-image.jpg")).Returns(noImageBlobClientMock.Object);
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(containerName)).Returns(blobContainerClientMock.Object);

        // Act
        var result = await _blobService.GetBlobAsync(blobName, containerName);

        // Assert
        Assert.Equal(expectedStream, result);
    }

    [Fact]
    public async Task GetBlobAsync_ShouldReturnStreamWithNoImage_WhenBlobNotExists()
    {
        // Arrange
        var containerName = "testcontainer";
        var blobName = "testblob";
        var noImageStream = new MemoryStream(Encoding.UTF8.GetBytes("no image content"));

        var blobClientMock = new Mock<BlobClient>();
        var noImageBlobClientMock = new Mock<BlobClient>();

        var responseMock = new Mock<Response<bool>>();
        responseMock.SetupGet(r => r.Value).Returns(false);
        blobClientMock.Setup(m => m.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(responseMock.Object);

        var noImageBlobDownloadInfo = BlobsModelFactory.BlobDownloadStreamingResult(content: noImageStream);
        var noImageDownloadResponse = Response.FromValue(noImageBlobDownloadInfo, new Mock<Response>().Object);
        noImageBlobClientMock.Setup(x => x.DownloadStreamingAsync(default, default)).ReturnsAsync(noImageDownloadResponse);

        var blobContainerClientMock = new Mock<BlobContainerClient>();
        blobContainerClientMock.Setup(x => x.GetBlobClient(blobName)).Returns(blobClientMock.Object);
        blobContainerClientMock.Setup(x => x.GetBlobClient("no-image.jpg")).Returns(noImageBlobClientMock.Object);

        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(containerName)).Returns(blobContainerClientMock.Object);

        // Act
        var result = await _blobService.GetBlobAsync(blobName, containerName);

        // Assert
        Assert.Equal(noImageStream, result);
    }

    [Fact]
    public async Task UploadImageAsync_UploadImage()
    {
        // Arrange
        var containerName = "testcontainer";
        var blobName = "testblob";
        var base64ImageContent = "data:image/jpeg;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAAADElEQVR42mP8/vY2DgAJIQO9I9U3AAAAAElFTkSuQmCC";

        var blobClientMock = new Mock<BlobClient>();
        var blobContainerClientMock = new Mock<BlobContainerClient>();

        blobContainerClientMock.Setup(x => x.GetBlobClient(blobName)).Returns(blobClientMock.Object);
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(containerName)).Returns(blobContainerClientMock.Object);

        var blobContentInfo = BlobsModelFactory.BlobContentInfo(ETag.All, DateTimeOffset.UtcNow, default, default, default, default, default);

        blobClientMock.Setup(x => x.UploadAsync(
            It.IsAny<Stream>(),
            It.Is<BlobUploadOptions>(o => o.HttpHeaders.ContentType == "image/jpeg"),
            It.IsAny<CancellationToken>()))
           .Returns(Task.FromResult(Response.FromValue(blobContentInfo, Mock.Of<Response>())))
           .Verifiable();

        // Act
        await _blobService.UploadImageAsync(blobName, base64ImageContent, containerName);

        // Assert
        blobClientMock.Verify();
    }
}
