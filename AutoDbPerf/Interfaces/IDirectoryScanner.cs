using System.Collections.Generic;
using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface IDirectoryScanner
    {
        IEnumerable<QueryInfo> ScanDirectories(string path);
    }
}