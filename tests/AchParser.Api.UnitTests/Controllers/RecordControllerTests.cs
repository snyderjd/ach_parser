using AchParser.Api.Controllers;
using AchParser.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;

public class RecordControllerTests
{
    [Fact]
    public async Task GetRecords_ReturnsAllRecords_WhenFileIdIsNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AchParserDbContext>().Options;
        var dbContextMock = new Mock<AchParserDbContext>(options);
        var controller = new RecordController(dbContextMock.Object);

        // Act
        var result = await controller.GetRecords(null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var records = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);

        Assert.Collection(records,
            r => Assert.Contains("Record X", r.ToString()),
            r => Assert.Contains("Record Y", r.ToString())
        );
    }

    [Fact]
    public async Task GetRecords_ReturnsRecordsForFile_WhenFileIdIsProvided()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AchParserDbContext>().Options;
        var dbContextMock = new Mock<AchParserDbContext>(options);
        var controller = new RecordController(dbContextMock.Object);
        int fileId = 42;

        // Act
        var result = await controller.GetRecords(fileId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var records = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
        
        Assert.Collection(records,
            r => Assert.Contains("Record A", r.ToString()),
            r => Assert.Contains("Record B", r.ToString())
        );
    }

    [Fact]
    public async Task GetRecord_ReturnsRecord_WhenRecordIdExists_WithoutFileId()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AchParserDbContext>().Options;
        var dbContextMock = new Mock<AchParserDbContext>(options);
        var controller = new RecordController(dbContextMock.Object);

        // Act
        var result = await controller.GetRecord(1, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Record X", okResult.Value.ToString());
    }

    [Fact]
    public async Task GetRecord_ReturnsRecord_WhenRecordIdAndFileIdExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AchParserDbContext>().Options;
        var dbContextMock = new Mock<AchParserDbContext>(options);
        var controller = new RecordController(dbContextMock.Object);

        // Act
        var result = await controller.GetRecord(1, 101);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Record A", okResult.Value.ToString());
    }

    [Fact]
    public async Task GetRecord_ReturnsNotFound_WhenRecordDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AchParserDbContext>().Options;
        var dbContextMock = new Mock<AchParserDbContext>(options);
        var controller = new RecordController(dbContextMock.Object);

        // Act
        var result = await controller.GetRecord(999, null);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}