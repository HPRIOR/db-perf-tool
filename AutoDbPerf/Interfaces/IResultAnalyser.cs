using System.Collections.Generic;
using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface IResultAnalyser
    {
        TableData AnalyseResults(IEnumerable<QueryResult2> results);
    }
}