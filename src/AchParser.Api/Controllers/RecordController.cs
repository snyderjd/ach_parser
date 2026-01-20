using AchParser.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace AchParser.Api.Controllers;

[ApiController]
[Route("api/records")]
[Route("api/files/{fileId}/records")]
public class RecordController : ControllerBase
{
    private readonly AchParserDbContext _dbContext;

    public RecordController(AchParserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetRecords([FromRoute] int? fileId)
    {
        var records = new List<object>();

        if (fileId.HasValue)
        {
            // Placeholder records for the given fileId
            records.Add(new { Id = 1, FileId = fileId.Value, Name = "Record A"});
            records.Add(new { Id = 2, FileId = fileId.Value, Name = "Record B"});
        }
        else
        {
            // All records
            records.Add(new { Id = 1, FileId = 101, Name = "Record X"});
            records.Add(new { Id = 2, FileId = 102, Name = "Record Y"});
        }

        return Ok(records);
    }
    
}

