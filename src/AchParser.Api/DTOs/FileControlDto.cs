namespace AchParser.Api.DTOs;

public class FileControlDto
{
    public Guid Id { get; set; }
    public int BatchCount { get; set; }
    public int BlockCount { get; set; }
    public int EntryAddendaCount { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public int LineNumber { get; set; }
    public string UnparsedRecord { get; set; } = null!;
}
