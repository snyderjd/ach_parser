using System;
using System.Collections.Generic;

namespace AchParser.Api.Models;

public class EntryDetail
{
    public Guid Id { get; set; }
    public Guid BatchHeaderId { get; set; }
    public string RoutingNumber { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public decimal Amount { get; set; }
    public string IndividualName { get; set; } = null!;
    public int LineNumber { get; set; }
    public string UnparsedRecord { get; set; } = null!;

    public BatchHeader BatchHeader { get; set; } = null!;
    public ICollection<Addenda> Addendas { get; set; } = new List<Addenda>();
}
