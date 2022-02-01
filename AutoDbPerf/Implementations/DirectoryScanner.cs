using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;

namespace AutoDbPerf.Implementations
{
    public class DirectoryScanner : IDirectoryScanner
    {
        public IEnumerable<ScenarioQueryPath> ScanDirectories(string path)
        {
            var scenarios = Directory
                .EnumerateDirectories(path)
                .Where(NotDotFile)
                .Where(ContainsNonDotFiles);
            

            return scenarios.OrderBy(x => x).Select(scenario =>
                new ScenarioQueryPath(scenario.Split(Path.DirectorySeparatorChar).Last(),
                    Directory.EnumerateFiles(scenario).Where(NotDotFile)));
        }

        private static bool NotDotFile(string str)
        {
            return !str.Split(Path.DirectorySeparatorChar).Last().StartsWith(".");
        }

        private static bool ContainsNonDotFiles(string str)
        {
            return Directory.EnumerateFiles(str).Where(NotDotFile).Any();
        }
    }
}