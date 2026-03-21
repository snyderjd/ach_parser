using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace AchParser.Api.Models;

public class Addenda
{
    public Guid Id { get; set; }
    [ForeignKey("EntryDetail")]
    public Guid? EntryDetailId { get; set; }
    public string Information { get; set; } = null!;
    public int LineNumber { get; set; }
    public string UnparsedRecord { get; set; } = null!;

    [InverseProperty("Addendas")]
    public EntryDetail? EntryDetail { get; set; }
}
