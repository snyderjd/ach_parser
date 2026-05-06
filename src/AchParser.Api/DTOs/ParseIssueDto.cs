namespace AchParser.Api.DTOs;

public class ParseIssueDto
{
    public string Severity { get; set; } = null!;
    public string Message { get; set; } = null!;
    public int? LineNumber { get; set; }
    public string? Code { get; set; }
}
