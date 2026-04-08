using System.Security.Cryptography;
using System.Text;
using AchParser.Api.Models;
using Microsoft.Extensions.Logging;

namespace AchParser.Api.Parsing;

public class AchFileParser : IAchFileParser
{
    private readonly ILogger<AchFileParser>? _logger;

    public AchFileParser(ILogger<AchFileParser>? logger = null)
    {
        _logger = logger;
    }

    public ParseResult Parse(string content, string fileName)
    {
        var issues = new List<ParseIssue>();

        if (content == null) content = string.Empty;

        var achFile = new AchFile
        {
            Filename = fileName,
            UnparsedFile = content
        };

        // compute hash
        using (var sha = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            achFile.Hash = BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
        }

        var lines = content.Replace("\r\n", "\n").Split('\n');

        BatchHeader? currentBatch = null;
        EntryDetail? lastEntry = null;

        for (int i = 0; i < lines.Length; i++)
        {
            var lineNumber = i + 1;
            var raw = lines[i];

            if (string.IsNullOrEmpty(raw))
            {
                // skip empty lines but record as info
                issues.Add(new ParseIssue(ParseSeverity.Info, "Empty line", lineNumber));
                continue;
            }

            if (raw.Length != 94)
            {
                issues.Add(new ParseIssue(ParseSeverity.Error, $"Unexpected record length: {raw.Length}", lineNumber));
                // attempt to continue
            }

            var recType = raw.Length > 0 ? raw[0] : '\0';

            try
            {
                switch (recType)
                {
                    case '1':
                        var fh = ParseFileHeader(raw, lineNumber);
                        achFile.FileHeaders!.Add(fh);
                        break;
                    case '5':
                        currentBatch = ParseBatchHeader(raw, lineNumber);
                        currentBatch.AchFile = achFile;
                        achFile.BatchHeaders!.Add(currentBatch);
                        lastEntry = null;
                        break;
                    case '6':
                        var entry = ParseEntryDetail(raw, lineNumber);
                        if (currentBatch == null)
                        {
                            issues.Add(new ParseIssue(ParseSeverity.Error, "EntryDetail encountered with no open batch", lineNumber));
                        }
                        else
                        {
                            entry.BatchHeader = currentBatch;
                            currentBatch.EntryDetails ??= new List<EntryDetail>();
                            currentBatch.EntryDetails.Add(entry);
                            lastEntry = entry;
                        }
                        break;
                    case '7':
                        var add = ParseAddenda(raw, lineNumber);
                        if (lastEntry == null)
                        {
                            issues.Add(new ParseIssue(ParseSeverity.Error, "Addenda encountered with no preceding entry", lineNumber));
                        }
                        else
                        {
                            add.EntryDetail = lastEntry;
                            lastEntry.Addendas ??= new List<Addenda>();
                            lastEntry.Addendas.Add(add);
                        }
                        break;
                    case '8':
                        var bc = ParseBatchControl(raw, lineNumber);
                        if (currentBatch == null)
                        {
                            issues.Add(new ParseIssue(ParseSeverity.Error, "BatchControl encountered with no open batch", lineNumber));
                        }
                        else
                        {
                            bc.BatchHeader = currentBatch;
                            currentBatch.BatchControls!.Add(bc);
                            currentBatch = null; // close batch
                            lastEntry = null;
                        }
                        break;
                    case '9':
                        var fc = ParseFileControl(raw, lineNumber);
                        fc.AchFile = achFile;
                        achFile.FileControls!.Add(fc);
                        break;
                    default:
                        issues.Add(new ParseIssue(ParseSeverity.Warning, $"Unknown record type '{recType}'", lineNumber));
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Exception parsing line {LineNumber}", lineNumber);
                issues.Add(new ParseIssue(ParseSeverity.Error, "Exception parsing record: " + ex.Message, lineNumber));
            }
        }

        // Basic structural validation
        if (!achFile.FileHeaders.Any())
        {
            issues.Add(new ParseIssue(ParseSeverity.Error, "Missing file header"));
            return new ParseResult(null, issues);
        }

        if (!achFile.FileControls.Any())
        {
            issues.Add(new ParseIssue(ParseSeverity.Error, "Missing file control"));
            return new ParseResult(null, issues);
        }

        return new ParseResult(achFile, issues);
    }

    private static FileHeader ParseFileHeader(string raw, int lineNumber)
    {
        // NACHA file header common fields (positions are 1-based):
        // 2-3 Priority
        // 4-13 Immediate Destination (10)
        // 14-23 Immediate Origin (10)
        // 24-29 File Creation Date (YYMMDD)
        // 30-33 File Creation Time (HHMM)
        // 74-93 Immediate Destination Name (23)

        var fh = new FileHeader
        {
            UnparsedRecord = raw,
            LineNumber = lineNumber,
            ImmediateDestination = SubstringSafe(raw, 3, 10).Trim(),
            ImmediateOrigin = SubstringSafe(raw, 13, 10).Trim(),
            ImmediateDestinationName = SubstringSafe(raw, 63, 23).Trim(),
            ImmediateOriginName = SubstringSafe(raw, 86, 23).Trim()
        };

        // parse date/time best-effort
        var dateStr = SubstringSafe(raw, 23, 6);
        if (DateTime.TryParseExact(dateStr, "yyMMdd", null, System.Globalization.DateTimeStyles.None, out var dt))
        {
            fh.FileCreationDate = dt.Date;
        }

        var timeStr = SubstringSafe(raw, 29, 4);
        if (TimeSpan.TryParseExact(timeStr, "hhmm", null, out var ts))
        {
            fh.FileCreationTime = ts;
        }

        return fh;
    }

    private static BatchHeader ParseBatchHeader(string raw, int lineNumber)
    {
        // positions: 2-4 Service Class Code (3), 5-20 Company Name (16), 41-50 Company Identification (10)
        var bh = new BatchHeader
        {
            UnparsedRecord = raw,
            LineNumber = lineNumber,
            ServiceClassCode = SubstringSafe(raw, 2, 3).Trim(),
            CompanyName = SubstringSafe(raw, 5, 16).Trim(),
            CompanyIdentification = SubstringSafe(raw, 40, 10).Trim()
        };

        return bh;
    }

    private static EntryDetail ParseEntryDetail(string raw, int lineNumber)
    {
        // positions: 4-12 RDFI routing (9) + check digit? NACHA routing is typically 9 digits at pos 4-12, account 13-29 (17), amount 30-39 (10)
        var routing = SubstringSafe(raw, 3, 9).Trim();
        var account = SubstringSafe(raw, 12, 17).Trim();
        var amountStr = SubstringSafe(raw, 29, 10).Trim();
        decimal amount = 0m;
        if (long.TryParse(amountStr, out var cents))
        {
            amount = cents / 100m;
        }

        var entry = new EntryDetail
        {
            UnparsedRecord = raw,
            LineNumber = lineNumber,
            RoutingNumber = routing,
            AccountNumber = account,
            Amount = amount,
            IndividualName = SubstringSafe(raw, 53, 22).Trim()
        };

        return entry;
    }

    private static Addenda ParseAddenda(string raw, int lineNumber)
    {
        // information is typically positions 9-88 (80)
        var info = SubstringSafe(raw, 8, 80).Trim();
        return new Addenda
        {
            UnparsedRecord = raw,
            LineNumber = lineNumber,
            Information = info
        };
    }

    private static BatchControl ParseBatchControl(string raw, int lineNumber)
    {
        // Entry/Addenda count pos 2-7 (6), total debit pos  debits credit pos etc. We'll parse best-effort.
        var countStr = SubstringSafe(raw, 1, 6).Trim();
        int count = 0;
        if (int.TryParse(countStr, out var c)) count = c;

        var debitStr = SubstringSafe(raw, 17, 12).Trim();
        var creditStr = SubstringSafe(raw, 29, 12).Trim();

        decimal debit = 0m, credit = 0m;
        if (long.TryParse(debitStr, out var d)) debit = d / 100m;
        if (long.TryParse(creditStr, out var cr)) credit = cr / 100m;

        return new BatchControl
        {
            UnparsedRecord = raw,
            LineNumber = lineNumber,
            EntryAddendaCount = count,
            TotalDebit = debit,
            TotalCredit = credit
        };
    }

    private static FileControl ParseFileControl(string raw, int lineNumber)
    {
        // Batch count pos 2-7 (6), block count 8-13 (6), entry/addenda count 14-21 (8), total debit 22-33 (12), total credit 34-45 (12)
        var batchCountStr = SubstringSafe(raw, 1, 6).Trim();
        var blockCountStr = SubstringSafe(raw, 7, 6).Trim();
        var entryCountStr = SubstringSafe(raw, 13, 8).Trim();
        var debitStr = SubstringSafe(raw, 21, 12).Trim();
        var creditStr = SubstringSafe(raw, 33, 12).Trim();

        int batchCount = 0, blockCount = 0, entryCount = 0;
        if (int.TryParse(batchCountStr, out var bc)) batchCount = bc;
        if (int.TryParse(blockCountStr, out var bl)) blockCount = bl;
        if (int.TryParse(entryCountStr, out var ec)) entryCount = ec;

        decimal debit = 0m, credit = 0m;
        if (long.TryParse(debitStr, out var d)) debit = d / 100m;
        if (long.TryParse(creditStr, out var cr)) credit = cr / 100m;

        return new FileControl
        {
            UnparsedRecord = raw,
            LineNumber = lineNumber,
            BatchCount = batchCount,
            BlockCount = blockCount,
            EntryAddendaCount = entryCount,
            TotalDebit = debit,
            TotalCredit = credit
        };
    }

    // Helper: substring using 0-based index, but parameters are 0-based here for convenience
    private static string SubstringSafe(string s, int startIndex0Based, int length)
    {
        if (s == null) return string.Empty;
        if (startIndex0Based < 0) startIndex0Based = 0;
        if (startIndex0Based >= s.Length) return string.Empty;
        var maxLen = Math.Min(length, s.Length - startIndex0Based);
        return s.Substring(startIndex0Based, maxLen);
    }
}
