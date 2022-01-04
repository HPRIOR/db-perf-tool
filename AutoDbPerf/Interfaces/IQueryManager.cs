using System.Collections.Generic;
using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface IQueryManager
    {
        public IEnumerable<QueryResult> GetQueryResult(string queryPath,
            int avgPrecision, int timeout = 5000);
    }
}