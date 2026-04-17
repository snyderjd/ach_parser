using AchParser.Api.Models;

namespace AchParser.Api.Parsing;

public interface IAchFileParser
{
    /// <summary>
    /// Parse a NACHA ACH file content into an AchFile domain model and produce parse issues.
    /// This method is synchronous by design per the PRD.
    /// </summary>
    
    ParseResult Parse(string content, string fileName);
}