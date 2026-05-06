namespace AchParser.Api.DTOs;

public class BatchHeaderDto
{
    public Guid Id { get; set; }
    public string ServiceClassCode { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string CompanyIdentification { get; set; } = null!;
    public int LineNumber { get; set; }
    public string UnparsedRecord { get; set; } = null!;
    public List<BatchControlDto> BatchControls { get; set; } = new();
    public List<EntryDetailDto> EntryDetails { get; set; } = new();
}
