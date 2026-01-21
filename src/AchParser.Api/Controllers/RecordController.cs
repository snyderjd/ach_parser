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

    [HttpGet("{recordId}")]
    public async Task<IActionResult> GetRecord([FromRoute] int recordId, [FromRoute] int? fileId)
    {
        object record = null;

        if (fileId.HasValue)
        {
            // Simulate fetching a record by fileId and recordId
            if (recordId == 1 && fileId == 101)
                record = new { Id = 1, FileId = 101, Name = "Record A" };
            else if (recordId == 2 && fileId == 101)
                record = new { Id = 2, FileId = 101, Name = "Record B" };
        }
        else
        {
            // Simulate fetching a record by recordId only
            if (recordId == 1)
                record = new { Id = 1, FileId = 101, Name = "Record X" };
            else if (recordId == 2)
                record = new { Id = 2, FileId = 102, Name = "Record Y" };
        }

        if (record == null)
            return NotFound();

        return Ok(record);        
    }

}

