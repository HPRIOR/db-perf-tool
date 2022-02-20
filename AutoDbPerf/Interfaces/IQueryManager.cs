using System.Collections.Generic;
using AutoDbPerf.Implementations;
using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface IQueryManager
    {
        public IEnumerable<QueryResult> GetQueryResults(IEnumerable<QueryInfo> queryInfo, int avgPrecision,
            int timeout = 5000);
    }
}