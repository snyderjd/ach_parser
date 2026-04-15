using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace AchParser.Api.Models;

public class EntryDetail
{
    public Guid Id { get; set; }
    [ForeignKey("BatchHeader")]
    public Guid? BatchHeaderId { get; set; }
    public string RoutingNumber { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public int TransactionCode { get; set; }
    public decimal Amount { get; set; }
    public string IndividualName { get; set; } = null!;
    public int LineNumber { get; set; }
    public string UnparsedRecord { get; set; } = null!;

    [InverseProperty("EntryDetails")]
    public BatchHeader? BatchHeader { get; set; }

    [InverseProperty("EntryDetail")]
    public ICollection<Addenda>? Addendas { get; set; }
}
