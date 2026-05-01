
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using AchParser.Api.Controllers;
using AchParser.Api.Data;
using AchParser.Api.DTOs;
using AchParser.Api.Models;
using AchParser.Api.Parsing;

using Xunit;

public class FileControllerTests
{
    private AchParserDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AchParserDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AchParserDbContext(options);
    }

    private static Mock<IAchFileParser> DefaultParserMock()
    {
        var mock = new Mock<IAchFileParser>();
        mock.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new ParseResult(null, new List<ParseIssue>()));
        return mock;
    }

    private FileController CreateController(AchParserDbContext dbContext, IAchFileParser? parser = null)
    {
        var resolvedParser = parser ?? DefaultParserMock().Object;
        return new FileController(dbContext, resolvedParser, NullLogger<FileController>.Instance);
    }

    [Fact]
    public async Task GetFiles_ReturnsOkResult_WithFiles()
    {
        // Arrange
        using var dbContext = GetInMemoryDbContext();
        dbContext.AchFiles.AddRange(new List<AchFile>
        {
            new AchFile { Id = Guid.NewGuid(), Filename = "file1.ach", Hash = "hash1", UnparsedFile = "content1", CreatedAt = DateTime.UtcNow },
            new AchFile { Id = Guid.NewGuid(), Filename = "file2.ach", Hash = "hash2", UnparsedFile = "content2", CreatedAt = DateTime.UtcNow }
        });
        await dbContext.SaveChangesAsync();
        var controller = CreateController(dbContext);

        // Act
        var result = await controller.GetFiles();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var files = Assert.IsAssignableFrom<List<AchFileResponseDto>>(okResult.Value);
        Assert.Equal(2, files.Count);
        Assert.Contains(files, f => f.FileName == "file1.ach");
        Assert.Contains(files, f => f.FileName == "file2.ach");
    }


    [Fact]
    public async Task UploadFile_PersistsFile_AndReturnsDto()
    {
        // Arrange
        using var dbContext = GetInMemoryDbContext();

        // Parser returns a successful result with a minimal parsed AchFile
        var parsedAchFile = new AchFile { Filename = "testfile.ach" };
        var successParser = new Mock<IAchFileParser>();
        successParser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new ParseResult(parsedAchFile, new List<ParseIssue>()));

        var controller = CreateController(dbContext, successParser.Object);
        var fileName = "testfile.ach";
        var fileContent = "test content";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
        var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };

        var uploadDto = new FileUploadDto { File = formFile };

        // Act
        var result = await controller.UploadFile(uploadDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var respDto = Assert.IsType<AchFileResponseDto>(createdResult.Value);
        Assert.Equal(fileName, respDto.FileName);
        Assert.Equal(fileContent, respDto.FileContent);
        Assert.NotEqual(Guid.Empty, respDto.Id);

        // Confirm file is in database
        var dbFile = dbContext.AchFiles.FirstOrDefault(f => f.Id == respDto.Id);
        Assert.NotNull(dbFile);
        Assert.Equal(fileContent, dbFile.UnparsedFile);
    }


    [Fact]
    public async Task GetFile_ReturnsOkResult_WithFile()
    {
        // Arrange
        using var dbContext = GetInMemoryDbContext();
        var achFile = new AchFile
        {
            Id = Guid.NewGuid(),
            Filename = "file1.ach",
            Hash = "hash1",
            UnparsedFile = "content1",
            CreatedAt = DateTime.UtcNow
        };
        dbContext.AchFiles.Add(achFile);
        await dbContext.SaveChangesAsync();
        var controller = CreateController(dbContext);

        // Act
        var result = await controller.GetFile(achFile.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<AchFileResponseDto>(okResult.Value);
        Assert.Equal(achFile.Id, dto.Id);
        Assert.Equal(achFile.Filename, dto.FileName);
        Assert.Equal(achFile.UnparsedFile, dto.FileContent);
    }

    [Fact]
    public async Task GetFile_ReturnsNotFound_WhenFileDoesNotExist()
    {
        // Arrange
        using var dbContext = GetInMemoryDbContext();
        var controller = CreateController(dbContext);

        // Act
        var result = await controller.GetFile(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
