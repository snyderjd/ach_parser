namespace AchParser.Api.DTOs;

public class FileHeaderDto
{
    public Guid Id { get; set; }
    public string ImmediateDestination { get; set; } = null!;
    public string ImmediateOrigin { get; set; } = null!;
    public DateTime FileCreationDate { get; set; }
    public TimeSpan FileCreationTime { get; set; }
    public string ImmediateDestinationName { get; set; } = null!;
    public string ImmediateOriginName { get; set; } = null!;
    public int LineNumber { get; set; }
    public string UnparsedRecord { get; set; } = null!;
}
