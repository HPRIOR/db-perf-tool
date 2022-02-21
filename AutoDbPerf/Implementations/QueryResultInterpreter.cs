using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations
{
    // rename to aggregator
    public class QueryResultInterpreter : IQueryResultInterpreter
    {
        public TableResult GetTableDataFrom(IEnumerable<QueryResult> queryResult)
        {
            var qrList = queryResult.ToList();

            var averageExecutionTime = qrList.Average(x => x.NumData[Data.EXECUTION_TIME]);
            var executionStdDev = qrList.Select(x => x.NumData[Data.EXECUTION_TIME]).StdDev();

            var numData = new Dictionary<Data, float>
            {
                { Data.AVG_EXECUTION_TIME, averageExecutionTime },
                { Data.EXECUTION_STD_DEV, executionStdDev }
            };

            return new TableResult(numData, null);
        }
    }
}