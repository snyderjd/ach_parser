using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AchParser.Api.Data;
using AchParser.Api.DTOs;
using AchParser.Api.Models;
using AchParser.Api.Parsing;

namespace AchParser.Api.Controllers;

[ApiController]
[Route("api/files")]
public class FileController : ControllerBase
{
    private readonly AchParserDbContext _dbContext;
    private readonly IAchFileParser _parser;
    private readonly ILogger<FileController> _logger;

    public FileController(AchParserDbContext dbContext, IAchFileParser parser, ILogger<FileController> logger)
    {
        _dbContext = dbContext;
        _parser = parser;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetFiles()
    {
        var files = await _dbContext.AchFiles
            .AsNoTracking()
            .Include(f => f.FileHeaders)
            .Include(f => f.FileControls)
            .Include(f => f.BatchHeaders!)
                .ThenInclude(b => b.BatchControls)
            .Include(f => f.BatchHeaders!)
                .ThenInclude(b => b.EntryDetails!)
                    .ThenInclude(e => e.Addendas)
            .ToListAsync();
        
        return Ok(files.Select(MapToDetailDto).ToList());
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetFile(Guid id)
    {
        var file = await _dbContext.AchFiles
            .AsNoTracking()
            .Include(f => f.FileHeaders)
            .Include(f => f.FileControls)
            .Include(f => f.BatchHeaders!)
                .ThenInclude(b => b.BatchControls)
            .Include(f => f.BatchHeaders!)
                .ThenInclude(b => b.EntryDetails!)
                    .ThenInclude(e => e.Addendas)
            .FirstOrDefaultAsync(f => f.Id == id);
        
        if (file == null)
            return NotFound();
        
        return Ok(MapToDetailDto(file));
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(AchFileResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFile([FromForm] FileUploadDto dto)
    {
        var file = dto.File;
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        string fileContent;
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            fileContent = await reader.ReadToEndAsync();
        }

        // Compute SHA256 hash of the file content
        string hash;
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(fileContent);
            var hashBytes = sha256.ComputeHash(bytes);
            hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLowerInvariant();
        }

        // B1: Always persist raw AchFile immediately.
        var achFile = new AchFile
        {
            Id = Guid.NewGuid(),
            Filename = file.FileName,
            Hash = hash,
            UnparsedFile = fileContent,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.AchFiles.Add(achFile);
        await _dbContext.SaveChangesAsync();

        // B2: Parse uploaded content.
        var parseResult = _parser.Parse(fileContent, file.FileName);

        // B3: Handle parse failures.
        if (!parseResult.Success)
        {
            _logger.LogWarning(
                "Parse failed for file {FileName} (id={FileId}). Issues: {IssueCount}",
                file.FileName,
                achFile.Id,
                parseResult.Issues.Count);

            foreach (var issue in parseResult.Issues.Where(i => i.Severity == ParseSeverity.Error))
            {
                _logger.LogError(
                    "Parse error in {FileName} at line {LineNumber}: {Message}",
                    file.FileName,
                    issue.LineNumber,
                    issue.Message);
            }

            var errorBody = new
            {
                fileId = achFile.Id,
                issues = parseResult.Issues.Select(i => new
                {
                    severity = i.Severity.ToString(),
                    message = i.Message,
                    lineNumber = i.LineNumber,
                    code = i.code
                }).ToList()
            };

            return BadRequest(errorBody);
        }

        // B4: Prepare parsed model — assign GUIDs and FK values for all nested entities.
        var parsedFile = parseResult.File!;
        PrepareChildEntities(achFile.Id, parsedFile);

        // B5: Persist parsed children.  EF Core wraps SaveChanges in an implicit transaction
        // so all children are committed atomically.  Navigation properties are already set by
        // PrepareChildEntities, so we can simply add the collections to the tracked context.
        if (parsedFile.FileHeaders != null)
            _dbContext.FileHeaders.AddRange(parsedFile.FileHeaders);

        if (parsedFile.FileControls != null)
            _dbContext.FileControls.AddRange(parsedFile.FileControls);

        if (parsedFile.BatchHeaders != null)
        {
            foreach (var bh in parsedFile.BatchHeaders)
            {
                _dbContext.BatchHeaders.Add(bh);

                if (bh.BatchControls != null)
                    _dbContext.BatchControls.AddRange(bh.BatchControls);

                if (bh.EntryDetails != null)
                {
                    foreach (var ed in bh.EntryDetails)
                    {
                        _dbContext.EntryDetails.Add(ed);

                        if (ed.Addendas != null)
                            _dbContext.Addendas.AddRange(ed.Addendas);
                    }
                }
            }
        }

        await _dbContext.SaveChangesAsync();

        // B6: Return 201 with AchFileResponseDto.
        var responseDto = new AchFileResponseDto
        {
            Id = achFile.Id,
            FileName = achFile.Filename,
            FileContent = achFile.UnparsedFile,
            OriginalFileName = achFile.Filename,
            UploadedAt = achFile.CreatedAt
        };

        return CreatedAtAction(nameof(GetFile), new { id = achFile.Id }, responseDto);
    }

    private static AchFileDetailDto MapToDetailDto(AchFile f) => new()
    {
        Id = f.Id,
        FileName = f.Filename,
        FileContent = f.UnparsedFile,
        OriginalFileName = f.Filename,
        UploadedAt = f.CreatedAt,
        FileHeaders = f.FileHeaders?.Select(MapToFileHeaderDto).ToList() ?? [],
        FileControls = f.FileControls?.Select(MapToFileControlDto).ToList() ?? [],
        BatchHeaders = f.BatchHeaders?.Select(MapToBatchHeaderDto).ToList() ?? []
    };

    private static FileHeaderDto MapToFileHeaderDto(FileHeader h) => new()
    {
        Id = h.Id,
        ImmediateDestination = h.ImmediateDestination,
        ImmediateOrigin = h.ImmediateOrigin,
        FileCreationDate = h.FileCreationDate,
        FileCreationTime = h.FileCreationTime,
        ImmediateDestinationName = h.ImmediateDestinationName,
        ImmediateOriginName = h.ImmediateOriginName,
        LineNumber = h.LineNumber,
        UnparsedRecord = h.UnparsedRecord
    };

    private static FileControlDto MapToFileControlDto(FileControl c) => new()
    {
        Id = c.Id,
        BatchCount = c.BatchCount,
        BlockCount = c.BlockCount,
        EntryAddendaCount = c.EntryAddendaCount,
        TotalDebit = c.TotalDebit,
        TotalCredit = c.TotalCredit,
        LineNumber = c.LineNumber,
        UnparsedRecord = c.UnparsedRecord
    };

    private static BatchHeaderDto MapToBatchHeaderDto(BatchHeader b) => new()
    {
        Id = b.Id,
        ServiceClassCode = b.ServiceClassCode,
        CompanyName = b.CompanyName,
        CompanyIdentification = b.CompanyIdentification,
        LineNumber = b.LineNumber,
        UnparsedRecord = b.UnparsedRecord,
        BatchControls = b.BatchControls?.Select(MapToBatchControlDto).ToList() ?? [],
        EntryDetails = b.EntryDetails?.Select(MapToEntryDetailDto).ToList() ?? []
    };

    private static BatchControlDto MapToBatchControlDto(BatchControl c) => new()
    {
        Id = c.Id,
        EntryAddendaCount = c.EntryAddendaCount,
        TotalDebit = c.TotalDebit,
        TotalCredit = c.TotalCredit,
        LineNumber = c.LineNumber,
        UnparsedRecord = c.UnparsedRecord
    };

    private static EntryDetailDto MapToEntryDetailDto(EntryDetail e) => new()
    {
        Id = e.Id,
        RoutingNumber = e.RoutingNumber,
        AccountNumber = e.AccountNumber,
        TransactionCode = e.TransactionCode,
        Amount = e.Amount,
        IndividualName = e.IndividualName,
        LineNumber = e.LineNumber,
        UnparsedRecord = e.UnparsedRecord,
        Addendas = e.Addendas?.Select(MapToAddendaDto).ToList() ?? []
    };

    private static AddendaDto MapToAddendaDto(Addenda a) => new()
    {
        Id = a.Id,
        Information = a.Information,
        LineNumber = a.LineNumber,
        UnparsedRecord = a.UnparsedRecord
    };

    /// <summary>
    /// Assigns new GUIDs and FK values to all parsed child entities before persistence.
    /// </summary>
    private static void PrepareChildEntities(Guid achFileId, AchFile parsedFile)
    {
        if (parsedFile.FileHeaders != null)
        {
            foreach (var fh in parsedFile.FileHeaders)
            {
                fh.Id = Guid.NewGuid();
                fh.AchFileId = achFileId;
            }
        }

        if (parsedFile.FileControls != null)
        {
            foreach (var fc in parsedFile.FileControls)
            {
                fc.Id = Guid.NewGuid();
                fc.AchFileId = achFileId;
            }
        }

        if (parsedFile.BatchHeaders != null)
        {
            foreach (var bh in parsedFile.BatchHeaders)
            {
                bh.Id = Guid.NewGuid();
                bh.AchFileId = achFileId;

                if (bh.BatchControls != null)
                {
                    foreach (var bc in bh.BatchControls)
                    {
                        bc.Id = Guid.NewGuid();
                        bc.BatchHeaderId = bh.Id;
                    }
                }

                if (bh.EntryDetails != null)
                {
                    foreach (var ed in bh.EntryDetails)
                    {
                        ed.Id = Guid.NewGuid();
                        ed.BatchHeaderId = bh.Id;

                        if (ed.Addendas != null)
                        {
                            foreach (var addenda in ed.Addendas)
                            {
                                addenda.Id = Guid.NewGuid();
                                addenda.EntryDetailId = ed.Id;
                            }
                        }
                    }
                }
            }
        }
    }
}