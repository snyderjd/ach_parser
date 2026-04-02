using System;

namespace AchParser.Generator.Utilities;

public static class RoutingNumberGenerator
{
    private static Random _rand = new Random();

    public static string Generate8Digits()
    {
        while (true)
        {
            var digits = _rand.Next(10000000, 99999999).ToString();
            var check = ComputeCheckDigit(digits);
            
            if (IsValid(digits + check.ToString()))
                return digits;
        }
    }

    public static int ComputeCheckDigit(string routing8)
    {
        int[] weights = {3,7,1,3,7,1,3,7};
        int sum = 0;
        
        for (int i = 0; i < 8; i++)
            sum += (routing8[i] - '0') * weights[i];
        
        int check = (10 - (sum % 10)) % 10;
        return check;
    }

    public static bool IsValid(string routing9)
    {
        if (routing9.Length != 9) return false;
        
        int sum = 0;
        int[] weights = {3,7,1,3,7,1,3,7,1};
        
        for (int i = 0; i < 9; i++)
            sum += (routing9[i] - '0') * weights[i];
        
        return sum % 10 == 0;
    }
}
