namespace AchParser.Api.Parsing;

public record ParseIssue(ParseSeverity Severity, string Message, int? LineNumber = null, string? code = null);