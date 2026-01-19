using AchParser.Api.Data;
using Microsoft.AspNetCore.Mvc;

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
        var files = new[]
        {
            new { file = "File1" },
            new { file = "File2" },
            new { file = "File3" }
        };
        
        return Ok(files);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetFile(int id)
    {
        // Simulate fetching a file by id
        var file = new { file = $"File{id}" };
        return Ok(file);
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile()
    {
        // Create a placeholder object for the file and return a 201
        var file = new { file = "NewFile" };
        return CreatedAtAction(nameof(UploadFile), file);
    }
}