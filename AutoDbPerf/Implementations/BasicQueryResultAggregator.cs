using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations
{
    public class BasicQueryResultAggregator : IQueryResultAggregator
    {
        public TableResult GetTableDataFrom(IEnumerable<QueryResult> queryResult)
        {
            var executionTime = queryResult.Select(x => x.NumData[Data.EXECUTION_TIME]).ToList();

            var averageExecutionTime = executionTime.Average();
            var executionStdDev = executionTime.StdDev();

            var minExecutionTime = executionTime.Min();
            var maxExecutionTime = executionTime.Max();

            var numData = new Dictionary<Data, float>
            {
                { Data.AVG_EXECUTION_TIME, averageExecutionTime },
                { Data.EXECUTION_STD_DEV, executionStdDev },
                { Data.MIN_EXECUTION_TIME, minExecutionTime },
                { Data.MAX_EXECUTION_TIME, maxExecutionTime }
            };

            return new TableResult(numData, null);
        }
    }
}