using AchParser.Api.Models;
using System.Linq;

namespace AchParser.Api.Parsing;

public record ParseResult(AchFile? File, IReadOnlyList<ParseIssue> Issues)
{
    public bool Success => File != null && !Issues.Any(i => i.Severity == ParseSeverity.Error);
}
