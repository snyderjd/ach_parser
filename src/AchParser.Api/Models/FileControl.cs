using System;

namespace AchParser.Api.Models;

public class FileControl
{
    public Guid Id { get; set; }
    public Guid AchFileId { get; set; }
    public int BatchCount { get; set; }
    public int BlockCount { get; set; }
    public int EntryAddendaCount { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public int LineNumber { get; set; }
    public string UnparsedRecord { get; set; } = null!;

    public AchFile AchFile { get; set; } = null!;
}
