using System;
using System.Linq;

namespace AchParser.Generator.Utilities
{
    public static class RandomUtil
    {
        private static Random _rand = new Random();
        private static string[] _names = new[] { "John Smith", "Jane Doe", "Alice Brown", "Bob Lee", "Chris Green", "Dana White", "Evan Black", "Fay Lin", "Gus Blue", "Holly Red" };
        public static string RandomDigits(int defaultLen, int min, int max)
        {
            int len = _rand.Next(min, max+1);
            return string.Concat(Enumerable.Range(0, len).Select(_ => _rand.Next(0,10).ToString()));
        }
        public static string RandomName()
        {
            return _names[_rand.Next(_names.Length)];
        }
    }
}
