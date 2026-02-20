
using AchParser.Api.Controllers;
using AchParser.Api.Data;
using AchParser.Api.DTOs;
using AchParser.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        var controller = new FileController(dbContext);

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
        var controller = new FileController(dbContext);
        var fileName = "testfile.ach";
        var fileContent = "test content";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
        var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };

        // Act
        var result = await controller.UploadFile(formFile);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var dto = Assert.IsType<AchFileResponseDto>(createdResult.Value);
        Assert.Equal(fileName, dto.FileName);
        Assert.Equal(fileContent, dto.FileContent);
        Assert.NotEqual(Guid.Empty, dto.Id);

        // Confirm file is in database
        var dbFile = dbContext.AchFiles.FirstOrDefault(f => f.Id == dto.Id);
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
        var controller = new FileController(dbContext);

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
        var controller = new FileController(dbContext);

        // Act
        var result = await controller.GetFile(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}