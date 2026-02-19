using System;

namespace AchParser.Api.Models;

public class Addenda
{
    public Guid Id { get; set; }
    public Guid EntryDetailId { get; set; }
    public string Information { get; set; } = null!;
    public int LineNumber { get; set; }
    public string UnparsedRecord { get; set; } = null!;

    public EntryDetail EntryDetail { get; set; } = null!;
}
