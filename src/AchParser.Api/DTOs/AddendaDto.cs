namespace AchParser.Api.DTOs;

public class AddendaDto
{
    public Guid Id { get; set; }
    public string Information { get; set; } = null!;
    public int LineNumber { get; set; }
    public string UnparsedRecord { get; set; } = null!;
}
