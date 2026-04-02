using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AchParser.Api.Models;

public class AchFile
{
    public Guid Id { get; set; }
    public string Filename { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public string UnparsedFile { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public ICollection<FileHeader>? FileHeaders { get; set; } = new List<FileHeader>();
    public ICollection<FileControl>? FileControls { get; set; } = new List<FileControl>();
    public ICollection<BatchHeader>? BatchHeaders { get; set; } = new List<BatchHeader>();
}
