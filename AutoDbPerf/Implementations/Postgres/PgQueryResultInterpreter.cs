using System;
using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations.Postgres
{
    public class PgQueryResultInterpreter : IQueryResultInterpreter
    {
        public TableResult GetTableDataFrom(IEnumerable<QueryResult> queryResult)
        {
            var qrList = queryResult.ToList();


            var averagePlanningTime = qrList.Average(x => x.NumData["PlanningTime"]);
            var planningStdDev = qrList.Select(x => x.NumData["PlanningTime"]).StdDev();
            var averageExecutionTime = qrList.Average(x => x.NumData["ExecutionTime"]);
            var executionStdDev = qrList.Select(x => x.NumData["ExecutionTime"]).StdDev();

            var numData = new Dictionary<string, float>
            {
                { "AvgPlanningTime", averagePlanningTime },
                { "PlanningStdDev", planningStdDev },
                { "AvgExecutionTime", averageExecutionTime },
                { "ExecutionStdDev", executionStdDev }
            };

            return new TableResult(numData, null);


        }
    }
}