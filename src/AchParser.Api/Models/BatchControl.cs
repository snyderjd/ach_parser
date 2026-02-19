using System;

namespace AchParser.Api.Models;

public class BatchControl
{
    public Guid Id { get; set; }
    public Guid BatchHeaderId { get; set; }
    public int EntryAddendaCount { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public int LineNumber { get; set; }
    public string UnparsedRecord { get; set; } = null!;

    public BatchHeader BatchHeader { get; set; } = null!;
}