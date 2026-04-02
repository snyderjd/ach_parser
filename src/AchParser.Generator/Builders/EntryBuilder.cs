using System;
using AchParser.Generator.Utilities;

namespace AchParser.Generator.Builders;

public static class EntryBuilder
{
    public static string Build(int traceSeq, out int routingHash, out int amount)
    {
        // Generate routing number (8 digits + check digit)
        var routing8 = RoutingNumberGenerator.Generate8Digits();
        var checkDigit = RoutingNumberGenerator.ComputeCheckDigit(routing8);
        var routingNumber = routing8 + checkDigit.ToString();
        routingHash = int.Parse(routing8);

        // Account number (8-17 digits)
        var accountNumber = RandomUtil.RandomDigits(12, 8, 17);

        // Amount (in cents, 10 digits)
        amount = AmountGenerator.GenerateAmount();
        var amountStr = amount.ToString().PadLeft(10, '0');

        // Individual name
        var name = RandomUtil.RandomName().PadRight(22);

        // Trace number (8 digit routing + 7 digit seq)
        var traceNumber = routing8 + traceSeq.ToString().PadLeft(7, '0');

        // Build entry detail record (Type 6)
        // 1: Record Type Code (6)
        // 2: Transaction Code (22)
        // 3: RDFI Routing (8)
        // 4: Check Digit (1)
        // 5: Account Number (17)
        // 6: Amount (10)
        // 7: Individual ID (15)
        // 8: Individual Name (22)
        // 9: Discretionary Data (2)
        // 10: Addenda Record Indicator (1)
        // 11: Trace Number (15)
        return $"6" +
            "22" +
            routing8 +
            checkDigit +
            accountNumber.PadRight(17) +
            amountStr +
            " ".PadRight(15) +
            name +
            "  " +
            "0" +
            traceNumber.PadLeft(15, '0');
    }
}
