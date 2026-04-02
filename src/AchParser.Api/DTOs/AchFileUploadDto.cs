using Microsoft.AspNetCore.Http;

namespace AchParser.Api.DTOs
{
    public class AchFileUploadDto
    {
        public IFormFile File { get; set; }
    }
}
