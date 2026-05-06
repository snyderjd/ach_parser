namespace AchParser.Api.DTOs;

public class AchFileDetailDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string FileContent { get; set; } = null!;
    public string OriginalFileName { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
    public List<FileHeaderDto> FileHeaders { get; set; } = new();
    public List<FileControlDto> FileControls { get; set; } = new();
    public List<BatchHeaderDto> BatchHeaders { get; set; } = new();
}
