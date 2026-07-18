using DangPhatFlex.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class FileUploadServiceTests
{
    [Fact]
    public async Task SaveAsync_WritesFileToUploadsSubfolder_AndReturnsRelativeUrl()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempRoot);
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(e => e.WebRootPath).Returns(tempRoot);

        var sut = new FileUploadService(envMock.Object);

        var content = "fake-image-bytes"u8.ToArray();
        using var stream = new MemoryStream(content);
        var file = new FormFile(stream, 0, content.Length, "file", "logo.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        var url = await sut.SaveAsync(file, "products");

        Assert.StartsWith("/uploads/products/", url);
        Assert.True(File.Exists(Path.Combine(tempRoot, url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar))));

        Directory.Delete(tempRoot, true);
    }
}
