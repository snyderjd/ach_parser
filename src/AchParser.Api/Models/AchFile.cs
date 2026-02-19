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

    public FileHeader FileHeader { get; set; } = null!;
    public FileControl FileControl { get; set; } = null!;
    public ICollection<BatchHeader> BatchHeaders { get; set; } = new List<BatchHeader>();
}
