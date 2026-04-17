using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AchParser.Api.Models;
using AchParser.Api.Parsing;
using Xunit;

namespace AchParser.Api.UnitTests.Parsing;

public class AchFileParserTests
{
    // Helper: build a 94-char NACHA record with fields placed at specific 0-based indices
    private static string BuildRecord(char recordType, params (int index, string text)[] fields)
    {
        var buf = Enumerable.Repeat(' ', 94).ToArray();
        buf[0] = recordType;
        foreach (var (index, text) in fields)
        {
            if (index < 0 || index >= 94) continue;
            for (int i = 0; i < text.Length && index + i < 94; i++) buf[index + i] = text[i];
        }
        return new string(buf);
    }

    [Fact]
    public void Parse_ValidSingleBatchFile_HappyPath()
    {
        var parser = new AchFileParser();

        // File header: immediate destination at index 3 (10), immediate origin at 13 (10), destination name at 63 (23), origin name at 86 (23)
        var fh = BuildRecord('1', (3, "DEST123456"), (13, "ORIG123456"), (63, "DEST NAME"), (86, "ORIG NAME"));

        // Batch header: service class code at 2 (3), company name at 5 (16), company id at 40 (10)
        var bh = BuildRecord('5', (2, "200"), (5, "ACME CORP"), (40, "CID1234567"));

        // Entry detail: tx code at 1 (2) -> 22 (credit), routing at 3 (9), account at 12 (17), amount at 29 (10) cents, individual name at 53 (22)
        var entryAmountCents = 12345; // $123.45
        var amountStr = entryAmountCents.ToString().PadLeft(10, '0');
        var ed = BuildRecord('6', (1, "22"), (3, "123456789"), (12, "00012345678901234"), (29, amountStr), (53, "JOHN DOE"));

        // Batch control: entry/addenda count at 1 (6) -> 1, debit at 17 (12) -> 0, credit at 29 (12) -> entryAmountCents
        var bc = BuildRecord('8', (1, "000001"), (17, "000000000000"), (29, entryAmountCents.ToString().PadLeft(12, '0')));

        // File control: batch count at 1 (6) -> 1, block count at 7 (6) -> 1, entry/addenda count at 13 (8) -> 1, total debit 21 (12) -> 0, total credit 33 (12)
        var fc = BuildRecord('9', (1, "000001"), (7, "000001"), (13, "00000001"), (21, "000000000000"), (33, entryAmountCents.ToString().PadLeft(12, '0')));

        var content = string.Join('\n', new[] { fh, bh, ed, bc, fc });

        var result = parser.Parse(content, "testfile.ach");

        Assert.True(result.Success);
        Assert.NotNull(result.File);
        var file = result.File!;
        Assert.Equal("testfile.ach", file.Filename);
        Assert.Single(file.FileHeaders!);
        Assert.Single(file.BatchHeaders!);
        Assert.Single(file.FileControls!);
        var batch = file.BatchHeaders!.First();
        Assert.Single(batch.EntryDetails!);
        var entry = batch.EntryDetails!.First();
        Assert.Equal("123456789", entry.RoutingNumber);
        Assert.Equal("00012345678901234", entry.AccountNumber);
        Assert.Equal(123.45m, entry.Amount);
    }

    [Fact]
    public void Parse_MultipleBatchesWithAddenda_ParsesCountsAndAddenda()
    {
        var parser = new AchFileParser();

        var fh = BuildRecord('1', (3, "DEST000000"), (13, "ORIG000000"));

        // Batch 1
        var bh1 = BuildRecord('5', (2, "200"), (5, "COMP A"), (40, "CID0000001"));
        // entry amount for $10.00 must be 10-digit cents field
        var ed1 = BuildRecord('6', (1, "27"), (3, "111111111"), (12, "ACCT00000000001"), (29, "0000001000"), (53, "ALICE")); // $10.00 debit
        var add1 = BuildRecord('7', (8, "INFORMATION A"));
        // batch control: entry/addenda count 2, debit 000000001000 (10.00), credit 0
        var bc1 = BuildRecord('8', (1, "000002"), (17, "000000001000"), (29, "000000000000"));

        // Batch 2
        var bh2 = BuildRecord('5', (2, "200"), (5, "COMP B"), (40, "CID0000002"));
        var ed2 = BuildRecord('6', (1, "22"), (3, "222222222"), (12, "ACCT00000000002"), (29, "0000002500"), (53, "BOB")); // $25.00 credit
        var add2 = BuildRecord('7', (8, "INFORMATION B"));
        // batch control: entry/addenda count 2, debit 0, credit 000000002500
        var bc2 = BuildRecord('8', (1, "000002"), (17, "000000000000"), (29, "000000002500"));

        // File control: batch count 2, block 1, entry/addenda count 4, total debit 10.00, total credit 25.00
        // file control: batchCount=2, blockCount=1, entry/addenda count 4, total debit=000000001000, total credit=000000002500
        var fc = BuildRecord('9', (1, "000002"), (7, "000001"), (13, "00000004"), (21, "000000001000"), (33, "000000002500"));

        var content = string.Join('\n', new[] { fh, bh1, ed1, add1, bc1, bh2, ed2, add2, bc2, fc });

        var result = parser.Parse(content, "multibatch.ach");

        Assert.DoesNotContain(result.Issues, (ParseIssue i) => i.Severity == ParseSeverity.Error);
        Assert.NotNull(result.File);
        var file = result.File!;
        Assert.Equal(2, file.BatchHeaders!.Count);
        // each entry has one addenda
        Assert.Equal(2, file.BatchHeaders!.SelectMany(b => b.EntryDetails!).Count());
        Assert.Single(file.FileControls!);
        // verify totals
        var totalDebit = file.BatchHeaders!.SelectMany(b => b.EntryDetails!).Where(e => e.TransactionCode == 27 || e.TransactionCode == 37).Sum(e => e.Amount);
        var totalCredit = file.BatchHeaders!.SelectMany(b => b.EntryDetails!).Where(e => e.TransactionCode == 22 || e.TransactionCode == 32).Sum(e => e.Amount);
        Assert.Equal(10.00m, totalDebit);
        Assert.Equal(25.00m, totalCredit);
    }

    [Fact]
    public void Parse_LineLengthMismatch_ProducesErrorIssue()
    {
        var parser = new AchFileParser();

        var fh = BuildRecord('1', (3, "DESTX"), (13, "ORIGX"));
        var bh = BuildRecord('5', (2, "200"), (5, "COMP"));
        // create a malformed short entry (too short)
        var badEntry = "6SHORT"; // length < 94
        var bc = BuildRecord('8', (1, "000001"), (17, "000000000000"), (29, "000000000000"));
        var fc = BuildRecord('9', (1, "000001"), (7, "000001"), (13, "00000001"), (21, "000000000000"), (33, "000000000000"));

        var content = string.Join('\n', new[] { fh, bh, badEntry, bc, fc });
        var result = parser.Parse(content, "badlength.ach");

        Assert.Contains(result.Issues, i => i.Severity == ParseSeverity.Error && i.Message.Contains("Unexpected record length"));
        Assert.False(result.Success);
    }

    [Fact]
    public void Parse_MissingFileHeaderOrControl_ReturnsNullFile()
    {
        var parser = new AchFileParser();

        // Missing file header
        var bh = BuildRecord('5', (2, "200"), (5, "COMP"));
        var fc = BuildRecord('9', (1, "000001"), (7, "000001"), (13, "00000000"), (21, "000000000000"), (33, "000000000000"));
        var contentNoHeader = string.Join('\n', new[] { bh, fc });
        var res1 = parser.Parse(contentNoHeader, "noheader.ach");
        Assert.Null(res1.File);
        Assert.Contains(res1.Issues, i => i.Severity == ParseSeverity.Error && i.Message.Contains("Missing file header"));

        // Missing file control
        var fh = BuildRecord('1', (3, "DEST"), (13, "ORIG"));
        var contentNoControl = string.Join('\n', new[] { fh, bh });
        var res2 = parser.Parse(contentNoControl, "nocontrol.ach");
        Assert.Null(res2.File);
        Assert.Contains(res2.Issues, i => i.Severity == ParseSeverity.Error && i.Message.Contains("Missing file control"));
    }

    [Fact]
    public void Parse_TotalsMismatch_ProducesError()
    {
        var parser = new AchFileParser();

        var fh = BuildRecord('1', (3, "DEST000000"), (13, "ORIG000000"));
        var bh = BuildRecord('5', (2, "200"), (5, "COMP"), (40, "CID"));
        var ed = BuildRecord('6', (1, "22"), (3, "123456789"), (12, "ACCT"), (29, "000000001000"), (53, "NAME")); // $10 credit
        // Batch control intentionally has wrong totals (credit 0)
        var bc = BuildRecord('8', (1, "000001"), (17, "000000000000"), (29, "000000000000"));
        var fc = BuildRecord('9', (1, "000001"), (7, "000001"), (13, "00000001"), (21, "000000000000"), (33, "000000000000"));

        var content = string.Join('\n', new[] { fh, bh, ed, bc, fc });
        var result = parser.Parse(content, "totals_mismatch.ach");

        Assert.Contains(result.Issues, i => i.Severity == ParseSeverity.Error && i.Message.Contains("Batch total credit mismatch") || i.Message.Contains("File total credit mismatch"));
        Assert.False(result.Success);
    }

    [Fact]
    public void Parse_Hash_CalculatedCorrectly()
    {
        var parser = new AchFileParser();
        var fh = BuildRecord('1', (3, "DEST123456"), (13, "ORIG123456"));
        // Add a minimal file control so parser returns a File object
        var fc = BuildRecord('9', (1, "000001"), (7, "000001"), (13, "00000000"), (21, "000000000000"), (33, "000000000000"));
        var content = string.Join('\n', new[] { fh, fh, fc });
        var result = parser.Parse(content, "hash.ach");

        Assert.NotNull(result.File);
        var expectedHash = ComputeSha256Hex(content);
        Assert.Equal(expectedHash, result.File!.Hash);
    }

    private static string ComputeSha256Hex(string content)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(content);
        return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
    }
}
