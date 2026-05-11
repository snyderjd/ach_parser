
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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

    // -------------------------------------------------------------------------
    // D1: Updated existing tests — GetFiles and GetFile now return AchFileDetailDto
    // -------------------------------------------------------------------------

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
        var files = Assert.IsAssignableFrom<List<AchFileDetailDto>>(okResult.Value);
        Assert.Equal(2, files.Count);
        Assert.Contains(files, f => f.FileName == "file1.ach");
        Assert.Contains(files, f => f.FileName == "file2.ach");
        // No children seeded — child collections must be empty
        Assert.All(files, f => Assert.Empty(f.FileHeaders));
        Assert.All(files, f => Assert.Empty(f.FileControls));
        Assert.All(files, f => Assert.Empty(f.BatchHeaders));
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
        var dto = Assert.IsType<AchFileDetailDto>(okResult.Value);
        Assert.Equal(achFile.Id, dto.Id);
        Assert.Equal(achFile.Filename, dto.FileName);
        Assert.Equal(achFile.UnparsedFile, dto.FileContent);
        // No children seeded — child collections must be empty
        Assert.Empty(dto.FileHeaders);
        Assert.Empty(dto.FileControls);
        Assert.Empty(dto.BatchHeaders);
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

    // -------------------------------------------------------------------------
    // D2: Upload success — parsed children are persisted
    // -------------------------------------------------------------------------

    [Fact]
    public async Task UploadFile_PersistsParsedChildren_WhenParserSucceeds()
    {
        // Arrange
        using var dbContext = GetInMemoryDbContext();

        var parsedAchFile = new AchFile
        {
            Filename = "parsed.ach",
            FileHeaders = new List<FileHeader>
            {
                new FileHeader
                {
                    ImmediateDestination = "091000019",
                    ImmediateOrigin = "123456789",
                    ImmediateDestinationName = "DEST BANK",
                    ImmediateOriginName = "ORIG BANK",
                    UnparsedRecord = "101 091000019 1234567890101010101A094101DEST BANK            ORIG BANK          "
                }
            },
            FileControls = new List<FileControl>
            {
                new FileControl
                {
                    BatchCount = 1,
                    BlockCount = 1,
                    EntryAddendaCount = 1,
                    TotalDebit = 0m,
                    TotalCredit = 100m,
                    UnparsedRecord = "9000001000001000000010000000000000000000001000000000000000"
                }
            },
            BatchHeaders = new List<BatchHeader>
            {
                new BatchHeader
                {
                    ServiceClassCode = "200",
                    CompanyName = "MY COMPANY",
                    CompanyIdentification = "1234567890",
                    UnparsedRecord = "5200MY COMPANY       1234567890CCDPAYROLL   0101010101A094",
                    BatchControls = new List<BatchControl>
                    {
                        new BatchControl
                        {
                            EntryAddendaCount = 1,
                            TotalDebit = 0m,
                            TotalCredit = 100m,
                            UnparsedRecord = "8200000001000000000000000000000010000000001234567890"
                        }
                    },
                    EntryDetails = new List<EntryDetail>
                    {
                        new EntryDetail
                        {
                            RoutingNumber = "091000019",
                            AccountNumber = "123456789",
                            TransactionCode = 22,
                            Amount = 100m,
                            IndividualName = "JOHN DOE",
                            UnparsedRecord = "6220910000191234567890       0000010000JOHN DOE              1234567890000001"
                        }
                    }
                }
            }
        };

        var successParser = new Mock<IAchFileParser>();
        successParser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new ParseResult(parsedAchFile, new List<ParseIssue>()));

        var controller = CreateController(dbContext, successParser.Object);
        var fileContent = "some ach content";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
        var formFile = new FormFile(stream, 0, stream.Length, "file", "parsed.ach")
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };

        // Act
        var result = await controller.UploadFile(new FileUploadDto { File = formFile });

        // Assert — response shape
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var respDto = Assert.IsType<AchFileResponseDto>(createdResult.Value);
        Assert.NotEqual(Guid.Empty, respDto.Id);

        // Assert — raw file persisted
        Assert.Equal(1, dbContext.AchFiles.Count());

        // Assert — parsed children linked to the raw file's id
        Assert.True(dbContext.FileHeaders.Any(h => h.AchFileId == respDto.Id));
        Assert.True(dbContext.FileControls.Any(c => c.AchFileId == respDto.Id));
        Assert.True(dbContext.BatchHeaders.Any(b => b.AchFileId == respDto.Id));

        // Assert — grandchildren linked to the batch header
        var batchHeader = dbContext.BatchHeaders.First(b => b.AchFileId == respDto.Id);
        Assert.True(dbContext.BatchControls.Any(bc => bc.BatchHeaderId == batchHeader.Id));
        Assert.True(dbContext.EntryDetails.Any(e => e.BatchHeaderId == batchHeader.Id));
    }

    // -------------------------------------------------------------------------
    // D3: Upload failure — only raw AchFile is persisted; controller returns 400
    // -------------------------------------------------------------------------

    [Fact]
    public async Task UploadFile_PersistsRawOnly_AndReturnsBadRequest_WhenParserReturnsNullFile()
    {
        // Arrange
        using var dbContext = GetInMemoryDbContext();

        var failParser = new Mock<IAchFileParser>();
        failParser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new ParseResult(null, new List<ParseIssue>()));

        var controller = CreateController(dbContext, failParser.Object);
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("bad content"));
        var formFile = new FormFile(stream, 0, stream.Length, "file", "bad.ach")
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };

        // Act
        var result = await controller.UploadFile(new FileUploadDto { File = formFile });

        // Assert — HTTP 400
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);

        // Serialize to JSON and use JsonDocument to inspect the anonymous body
        var json = JsonSerializer.Serialize(badRequest.Value);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("fileId", out var fileIdProp));
        Assert.True(Guid.TryParse(fileIdProp.GetString(), out var fileId));
        Assert.NotEqual(Guid.Empty, fileId);

        Assert.True(root.TryGetProperty("issues", out var issuesProp));
        Assert.Equal(0, issuesProp.GetArrayLength());

        // Assert — only the raw AchFile was persisted; no child rows
        Assert.Equal(1, dbContext.AchFiles.Count());
        Assert.Equal(0, dbContext.FileHeaders.Count());
        Assert.Equal(0, dbContext.BatchHeaders.Count());
    }

    [Fact]
    public async Task UploadFile_PersistsRawOnly_AndReturnsBadRequest_WhenParserReturnsErrorIssue()
    {
        // Arrange
        using var dbContext = GetInMemoryDbContext();

        var issues = new List<ParseIssue>
        {
            new ParseIssue(ParseSeverity.Error, "Missing file header", 1)
        };
        var failParser = new Mock<IAchFileParser>();
        failParser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new ParseResult(new AchFile { Filename = "bad.ach" }, issues));

        var controller = CreateController(dbContext, failParser.Object);
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("bad content"));
        var formFile = new FormFile(stream, 0, stream.Length, "file", "bad.ach")
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };

        // Act
        var result = await controller.UploadFile(new FileUploadDto { File = formFile });

        // Assert — HTTP 400
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);

        // Serialize to JSON and use JsonDocument to inspect the anonymous body
        var json = JsonSerializer.Serialize(badRequest.Value);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("fileId", out var fileIdProp));
        Assert.True(Guid.TryParse(fileIdProp.GetString(), out var fileId));
        Assert.NotEqual(Guid.Empty, fileId);

        Assert.True(root.TryGetProperty("issues", out var issuesProp));
        Assert.Equal(1, issuesProp.GetArrayLength());
        var firstIssue = issuesProp[0];
        Assert.Equal("Error", firstIssue.GetProperty("severity").GetString());
        Assert.Equal("Missing file header", firstIssue.GetProperty("message").GetString());

        // Assert — only the raw AchFile was persisted; no child rows
        Assert.Equal(1, dbContext.AchFiles.Count());
        Assert.Equal(0, dbContext.FileHeaders.Count());
        Assert.Equal(0, dbContext.BatchHeaders.Count());
    }

    // -------------------------------------------------------------------------
    // D4: Retrieval — parsed associations are included in response when present
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetFiles_ReturnsParsedAssociations_WhenPresent()
    {
        // Arrange
        using var dbContext = GetInMemoryDbContext();
        var fileId = Guid.NewGuid();
        var batchHeaderId = Guid.NewGuid();

        dbContext.AchFiles.Add(new AchFile
        {
            Id = fileId,
            Filename = "full.ach",
            Hash = "h",
            UnparsedFile = "c",
            CreatedAt = DateTime.UtcNow
        });
        dbContext.FileHeaders.Add(new FileHeader
        {
            Id = Guid.NewGuid(),
            AchFileId = fileId,
            ImmediateDestination = "091000019",
            ImmediateOrigin = "123456789",
            ImmediateDestinationName = "DEST BANK",
            ImmediateOriginName = "ORIG BANK",
            UnparsedRecord = "r"
        });
        dbContext.BatchHeaders.Add(new BatchHeader
        {
            Id = batchHeaderId,
            AchFileId = fileId,
            ServiceClassCode = "200",
            CompanyName = "CO",
            CompanyIdentification = "id",
            UnparsedRecord = "r"
        });
        dbContext.BatchControls.Add(new BatchControl
        {
            Id = Guid.NewGuid(),
            BatchHeaderId = batchHeaderId,
            UnparsedRecord = "r"
        });
        dbContext.EntryDetails.Add(new EntryDetail
        {
            Id = Guid.NewGuid(),
            BatchHeaderId = batchHeaderId,
            RoutingNumber = "091000019",
            AccountNumber = "123456789",
            IndividualName = "JOHN DOE",
            TransactionCode = 22,
            UnparsedRecord = "r"
        });
        await dbContext.SaveChangesAsync();

        var controller = CreateController(dbContext);

        // Act
        var result = await controller.GetFiles();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var files = Assert.IsAssignableFrom<List<AchFileDetailDto>>(okResult.Value);
        Assert.Single(files);

        var dto = files[0];
        Assert.Equal(fileId, dto.Id);
        Assert.Single(dto.FileHeaders);
        Assert.Empty(dto.FileControls);
        Assert.Single(dto.BatchHeaders);
        Assert.Single(dto.BatchHeaders[0].BatchControls);
        Assert.Single(dto.BatchHeaders[0].EntryDetails);
    }

    [Fact]
    public async Task GetFile_ReturnsParsedAssociations_WhenPresent()
    {
        // Arrange
        using var dbContext = GetInMemoryDbContext();
        var fileId = Guid.NewGuid();
        var batchHeaderId = Guid.NewGuid();

        dbContext.AchFiles.Add(new AchFile
        {
            Id = fileId,
            Filename = "full.ach",
            Hash = "h",
            UnparsedFile = "c",
            CreatedAt = DateTime.UtcNow
        });
        dbContext.FileHeaders.Add(new FileHeader
        {
            Id = Guid.NewGuid(),
            AchFileId = fileId,
            ImmediateDestination = "091000019",
            ImmediateOrigin = "123456789",
            ImmediateDestinationName = "DEST BANK",
            ImmediateOriginName = "ORIG BANK",
            UnparsedRecord = "r"
        });
        dbContext.BatchHeaders.Add(new BatchHeader
        {
            Id = batchHeaderId,
            AchFileId = fileId,
            ServiceClassCode = "200",
            CompanyName = "CO",
            CompanyIdentification = "id",
            UnparsedRecord = "r"
        });
        dbContext.BatchControls.Add(new BatchControl
        {
            Id = Guid.NewGuid(),
            BatchHeaderId = batchHeaderId,
            UnparsedRecord = "r"
        });
        dbContext.EntryDetails.Add(new EntryDetail
        {
            Id = Guid.NewGuid(),
            BatchHeaderId = batchHeaderId,
            RoutingNumber = "091000019",
            AccountNumber = "123456789",
            IndividualName = "JOHN DOE",
            TransactionCode = 22,
            UnparsedRecord = "r"
        });
        await dbContext.SaveChangesAsync();

        var controller = CreateController(dbContext);

        // Act
        var result = await controller.GetFile(fileId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<AchFileDetailDto>(okResult.Value);
        Assert.Equal(fileId, dto.Id);
        Assert.Single(dto.FileHeaders);
        Assert.Empty(dto.FileControls);
        Assert.Single(dto.BatchHeaders);
        Assert.Single(dto.BatchHeaders[0].BatchControls);
        Assert.Single(dto.BatchHeaders[0].EntryDetails);
    }
}
