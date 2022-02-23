using System;
using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations.Postgres
{
    public class PgQueryResultAggregator : IQueryResultAggregator
    {
        public TableResult GetTableDataFrom(IEnumerable<QueryResult> queryResult)
        {
            var qrList = queryResult.ToList();

            var planningTimes = qrList.Select(x => x.NumData[Data.PLANNING_TIME]).ToList();
            var executionTimes = qrList.Select(x => x.NumData[Data.EXECUTION_TIME]).ToList();

            var averagePlanningTime = planningTimes.Average();
            var planningStdDev = planningTimes.StdDev();
            var minPlanningTime = planningTimes.Min();
            var maxPlanningTime = planningTimes.Max();

            var averageExecutionTime = executionTimes.Average();
            var executionStdDev = executionTimes.StdDev();
            var minExecutionTime = executionTimes.Min();
            var maxExecutionTime = executionTimes.Max();

            var numData = new Dictionary<Data, float>
            {
                { Data.AVG_PLANNING_TIME, averagePlanningTime },
                { Data.PLANNING_STD_DEV, planningStdDev },
                { Data.MIN_PLANNING_TIME, minPlanningTime },
                { Data.MAX_PLANNING_TIME, maxPlanningTime },
                { Data.AVG_EXECUTION_TIME, averageExecutionTime },
                { Data.EXECUTION_STD_DEV, executionStdDev },
                { Data.MIN_EXECUTION_TIME, minExecutionTime },
                { Data.MAX_EXECUTION_TIME, maxExecutionTime },
            };

            return new TableResult(numData, null);
        }
    }
}