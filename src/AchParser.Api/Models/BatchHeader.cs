using System;
using System.Collections.Generic;

namespace AchParser.Api.Models;

public class BatchHeader
{
    public Guid Id { get; set; }
    public Guid AchFileId { get; set; }
    public string ServiceClassCode { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string CompanyIdentification { get; set; } = null!;
    public int LineNumber { get; set; }
    public string UnparsedRecord { get; set; } = null!;

    public AchFile AchFile { get; set; } = null!;
    public BatchControl BatchControl { get; set; } = null!;
    public ICollection<EntryDetail> EntryDetails { get; set; } = new List<EntryDetail>();
}
