using AchParser.Api.Data;
using AchParser.Api.DTOs;
using AchParser.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AchParser.Api.Controllers;

[ApiController]
[Route("api/files")]
public class FileController : ControllerBase
{
    private readonly AchParserDbContext _dbContext;

    public FileController(AchParserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetFiles()
    {
        var files = await _dbContext.AchFiles
            .AsNoTracking()
            .Select(f => new AchFileResponseDto
            {
                Id = f.Id,
                FileName = f.Filename,
                FileContent = f.UnparsedFile,
                OriginalFileName = f.Filename,
                UploadedAt = f.CreatedAt
            })
            .ToListAsync();
        return Ok(files);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetFile(Guid id)
    {
        var file = await _dbContext.AchFiles
            .AsNoTracking()
            .Where(f => f.Id == id)
            .Select(f => new AchFileResponseDto
            {
                Id = f.Id,
                FileName = f.Filename,
                FileContent = f.UnparsedFile,
                OriginalFileName = f.Filename,
                UploadedAt = f.CreatedAt
            })
            .FirstOrDefaultAsync();
        if (file == null)
            return NotFound();
        return Ok(file);
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
    {
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
}