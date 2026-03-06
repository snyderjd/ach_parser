using System;
using System;

namespace AchParser.Generator.Formatting
{
    public static class FixedWidthFormatter
    {
        public static string FormatFileHeader(DateTime now)
        {
            // Type 1: File Header
            // 1: Record Type Code (1)
            // 2: Priority Code (2)
            // 3: Immediate Dest (10)
            // 4: Immediate Origin (10)
            // 5: File Creation Date (6)
            // 6: File Creation Time (4)
            // 7: File ID Modifier (1)
            // 8: Record Size (3)
            // 9: Blocking Factor (2)
            // 10: Format Code (1)
            // 11: Immediate Dest Name (23)
            // 12: Immediate Origin Name (23)
            // 13: Reference Code (8)
            return $"1" +
                "01" +
                " 123456789 " +
                " 987654321 " +
                now.ToString("yyMMdd") +
                now.ToString("HHmm") +
                "A" +
                "094" +
                "10" +
                "1" +
                "DEST BANK NAME".PadRight(23) +
                "ORIGIN BANK NAME".PadRight(23) +
                "REFCODE ";
        }

        public static string FormatBatchHeader(DateTime now)
        {
            // Type 5: Batch Header
            // 1: Record Type Code (1)
            // 2: Service Class Code (3)
            // 3: Company Name (16)
            // 4: Company Discretionary Data (20)
            // 5: Company ID (10)
            // 6: SEC Code (3)
            // 7: Company Entry Desc (10)
            // 8: Effective Entry Date (6)
            // 9: Settlement Date (3)
            // 10: Originator Status Code (1)
            // 11: ODFI ID (8)
            // 12: Batch Number (7)
            return $"5" +
                "220" +
                "TEST COMPANY".PadRight(16) +
                " ".PadRight(20) +
                "1234567890" +
                "PPD" +
                "PAYROLL   " +
                now.ToString("yyMMdd") +
                "   " +
                "1" +
                "87654321" +
                "0000001";
        }

        public static string FormatBatchControl(int entryCount, long entryHash, long totalCredit)
        {
            // Type 8: Batch Control
            // 1: Record Type Code (1)
            // 2: Service Class Code (3)
            // 3: Entry/Addenda Count (6)
            // 4: Entry Hash (10)
            // 5: Total Debit (12)
            // 6: Total Credit (12)
            // 7: Company ID (10)
            // 8: Message Auth Code (19)
            // 9: Reserved (6)
            // 10: ODFI ID (8)
            // 11: Batch Number (7)
            return $"8" +
                "220" +
                entryCount.ToString().PadLeft(6, '0') +
                (entryHash % 10000000000).ToString().PadLeft(10, '0') +
                "0".PadLeft(12, '0') +
                totalCredit.ToString().PadLeft(12, '0') +
                "1234567890" +
                " ".PadRight(19) +
                " ".PadRight(6) +
                "87654321" +
                "0000001";
        }

        public static string FormatFileControl(int batchCount, int entryCount, long entryHash, long totalCredit)
        {
            // Type 9: File Control
            // 1: Record Type Code (1)
            // 2: Batch Count (6)
            // 3: Block Count (6)
            // 4: Entry/Addenda Count (8)
            // 5: Entry Hash (10)
            // 6: Total Debit (12)
            // 7: Total Credit (12)
            // 8: Reserved (39)
            int totalRecords = 1 + 1 + entryCount + 1 + 1; // header, batch header, entries, batch control, file control
            int blockCount = (int)Math.Ceiling(totalRecords / 10.0);
            return $"9" +
                batchCount.ToString().PadLeft(6, '0') +
                blockCount.ToString().PadLeft(6, '0') +
                entryCount.ToString().PadLeft(8, '0') +
                (entryHash % 10000000000).ToString().PadLeft(10, '0') +
                "0".PadLeft(12, '0') +
                totalCredit.ToString().PadLeft(12, '0') +
                " ".PadRight(39);
        }
    }
}
