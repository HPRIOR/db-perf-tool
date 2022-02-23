using System.Collections.Generic;
using AutoDbPerf.Implementations;
using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface IQueryResultsAnalyser
    {
        TableData GetTableData(IEnumerable<QueryResult> queryResults);
    }
}