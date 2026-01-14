using Microsoft.EntityFrameworkCore;

namespace AchParser.Api.Data;

public class AchParserDbContext : DbContext
{
    public AchParserDbContext(DbContextOptions<AchParserDbContext> options) : base(options)
    {

    }
}