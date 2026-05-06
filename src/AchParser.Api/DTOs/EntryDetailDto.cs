namespace AchParser.Api.DTOs;

public class EntryDetailDto
{
    public Guid Id { get; set; }
    public string RoutingNumber { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public int TransactionCode { get; set; }
    public decimal Amount { get; set; }
    public string IndividualName { get; set; } = null!;
    public int LineNumber { get; set; }
    public string UnparsedRecord { get; set; } = null!;
    public List<AddendaDto> Addendas { get; set; } = new();
}
