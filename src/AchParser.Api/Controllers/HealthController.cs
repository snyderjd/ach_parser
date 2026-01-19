using AchParser.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace AchParser.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly AchParserDbContext _dbContext;

    public HealthController(AchParserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> CheckDatabaseHealth()
    {
        var canConnect = await _dbContext.Database.CanConnectAsync();
        return Ok(new { database = canConnect });
    }
}