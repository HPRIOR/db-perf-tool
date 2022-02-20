using System.IO;
using System.Linq;

namespace AutoDbPerf.Utils
{
    public static class StringUtils
    {
        public static string GetScenarioFromPath(this string path)
        {
            return path.Split(Path.DirectorySeparatorChar)[^2];
        }

        public static string GetQueryNameFromPath(this string path)
        {
            return path.Split(Path.DirectorySeparatorChar).Last().Split(".").First();
        }

        public static string MultiplyBy(this string str, int num, string separator = "")
        {
            if (num == 0)
                return "";
            return Enumerable
                .Range(0, num)
                .Select(_ => str)
                .Aggregate((a, b) => $"{a}{separator}{b}");
        }
    }
}