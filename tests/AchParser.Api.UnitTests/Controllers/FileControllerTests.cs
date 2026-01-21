using AchParser.Api.Controllers;
using AchParser.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Threading.Tasks;

public class FileControllerTests
{
    [Fact]
    public async Task GetFiles_ReturnsOkResult_WithFiles()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AchParserDbContext>().Options;
        var dbContextMock = new Mock<AchParserDbContext>(options);
        var controller = new FileController(dbContextMock.Object);

        // Act
        var result = await controller.GetFiles();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var files = Assert.IsAssignableFrom<object[]>(okResult.Value);
        Assert.Equal(3, files.Length);
    }

    [Fact]
    public async Task UploadFile_ReturnsCreatedAtActionResult_WithFile()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AchParserDbContext>().Options;
        var dbContextMock = new Mock<AchParserDbContext>(options);
        var controller = new FileController(dbContextMock.Object);

        // Act
        var result = await controller.UploadFile();

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(controller.UploadFile), createdResult.ActionName);
        var file = createdResult.Value;
        
        Assert.NotNull(file);
        Assert.Equal("NewFile", file.GetType().GetProperty("file")?.GetValue(file));
    }

    [Fact]
    public async Task GetFile_ReturnsOkResult_WithFile()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AchParserDbContext>().Options;
        var dbContextMock = new Mock<AchParserDbContext>(options);
        var controller = new FileController(dbContextMock.Object);
        int testId = 2;

        // Act
        var result = await controller.GetFile(testId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var file = okResult.Value;

        Assert.NotNull(file);
        Assert.Equal($"File{testId}", file.GetType().GetProperty("file")?.GetValue(file));
    }
}