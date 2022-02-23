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


            var averagePlanningTime = qrList.Average(x => x.NumData[Data.PLANNING_TIME]);
            var planningStdDev = qrList.Select(x => x.NumData[Data.PLANNING_TIME]).StdDev();
            var averageExecutionTime = qrList.Average(x => x.NumData[Data.EXECUTION_TIME]);
            var executionStdDev = qrList.Select(x => x.NumData[Data.EXECUTION_TIME]).StdDev();

            var numData = new Dictionary<Data, float>
            {
                { Data.AVG_PLANNING_TIME, averagePlanningTime },
                { Data.PLANNING_STD_DEV, planningStdDev },
                { Data.AVG_EXECUTION_TIME, averageExecutionTime },
                { Data.EXECUTION_STD_DEV, executionStdDev }
            };

            return new TableResult(numData, null);


        }
    }
}