using System;
using System.Collections.Generic;

namespace AchParser.Api.Models;

public class AchFile
{
    public Guid Id { get; set; }
    public string Filename { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public string UnparsedFile { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public FileHeader? FileHeader { get; set; }
    public FileControl? FileControl { get; set; }
    public ICollection<BatchHeader> BatchHeaders { get; set; } = new List<BatchHeader>();
}
