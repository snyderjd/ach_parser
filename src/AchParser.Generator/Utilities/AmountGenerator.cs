using System;

namespace AchParser.Generator.Utilities
{
    public static class AmountGenerator
    {
        private static Random _rand = new Random();
        public static int GenerateAmount()
        {
            // $1.00 to $10,000.00 in cents
            return _rand.Next(100, 1000000);
        }
    }
}
