using System;

namespace AchParser.Api.DTOs
{
    public class AchFileResponseDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileContent { get; set; }
        public string OriginalFileName { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
