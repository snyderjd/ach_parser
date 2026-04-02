using System;
using System.Collections.Generic;
using System.Text;
using AchParser.Generator.Builders;
using AchParser.Generator.Formatting;
using AchParser.Generator.Utilities;

namespace AchParser.Generator.Builders;

public class AchFileBuilder
{
    public string Build(int entryCount)
    {
        var sb = new StringBuilder();
        var now = DateTime.Now;
        var fileHeader = FixedWidthFormatter.FormatFileHeader(now);
        sb.AppendLine(fileHeader);

        var batchHeader = FixedWidthFormatter.FormatBatchHeader(now);
        sb.AppendLine(batchHeader);

        var entries = new List<string>();
        var entryHash = 0L;
        var totalCredit = 0L;

        for (int i = 0; i < entryCount; i++)
        {
            var entry = EntryBuilder.Build(i + 1, out int routingHash, out int amount);
            entries.Add(entry);
            entryHash += routingHash;
            totalCredit += amount;
        }

        foreach (var entry in entries)
            sb.AppendLine(entry);

        var batchControl = FixedWidthFormatter.FormatBatchControl(entryCount, entryHash, totalCredit);
        sb.AppendLine(batchControl);

        var fileControl = FixedWidthFormatter.FormatFileControl(1, entryCount, entryHash, totalCredit);
        sb.AppendLine(fileControl);

        // Padding to block size (10 records)
        int totalRecords = 1 + 1 + entryCount + 1 + 1; // header, batch header, entries, batch control, file control
        int padNeeded = (10 - (totalRecords % 10)) % 10;

        for (int i = 0; i < padNeeded; i++)
            sb.AppendLine(new String('9', 94));

        return sb.ToString();
    }
}