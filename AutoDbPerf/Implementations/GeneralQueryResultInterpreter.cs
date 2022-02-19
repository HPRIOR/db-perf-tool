using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations
{
    public class GeneralQueryResultInterpreter : IQueryResultInterpreter
    {
        public TableResult GetTableDataFrom(IEnumerable<QueryResult> queryResult)
        {
            var qrList = queryResult.ToList();


            var averageExecutionTime = qrList.Average(x => x.NumData["ExecutionTime"]);
            var executionStdDev = qrList.Select(x => x.NumData["ExecutionTime"]).StdDev();

            var numData = new Dictionary<string, float>
            {
                { "AvgExecutionTime", averageExecutionTime },
                { "ExecutionStdDev", executionStdDev }
            };

            return new TableResult(numData, null);
        }
    }
}