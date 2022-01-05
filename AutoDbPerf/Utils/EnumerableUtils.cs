using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;

namespace AutoDbPerf.Utils
{
    public static class EnumerableUtils
    {
        private static string FlattenStringBase(this IEnumerable<string> strings, string separator) =>
             strings.Aggregate((a, b) => a + separator + b);
        
        public static string FlattenToParagraph(this IEnumerable<string> strings)
        {
            return FlattenStringBase(strings, "\n");
        }
        
        public static string FlattenToCommaList(this IEnumerable<string> strings)
        {
            return FlattenStringBase(strings, ",");
        }

        public static float GetFirstNumberFromLineWith(this IEnumerable<string> strings, string identifier)
        {
            try
            {
                return float.Parse(strings
                    .First(str => str.Contains(identifier)).Split()
                    .First(str => str.Any(char.IsDigit)));
            }
            catch (InvalidOperationException)
            {
                return 0;
            }
        }
        
        public static float GetNumberFromLineWithoutSpaces(this IEnumerable<string> strings, string identifier)
        {
            try
            {
                var line = strings.First(str => str.Contains(identifier));
                var str = new string(line.Where(char.IsDigit).ToArray());
                return float.Parse(str);

            }
            catch (InvalidOperationException)
            {
                return 0;
            }
        }

        public static IEnumerable<T> AllButFirst<T>(this IEnumerable<T> ts) => ts.ToArray()[1..];
    }
}