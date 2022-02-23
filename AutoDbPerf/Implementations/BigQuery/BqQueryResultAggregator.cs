using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations.BigQuery
{
    //TODO add tests for the various aggregators
    public class BqQueryResultAggregator : IQueryResultAggregator
    {
        public TableResult GetTableDataFrom(IEnumerable<QueryResult> queryResult)
        {
            var qrList = queryResult.ToList();
            var executionTime = qrList.Select(x => x.NumData[Data.EXECUTION_TIME]).ToList();
            var bytesProcessed = qrList.Select(x => x.NumData[Data.BYTES_PROCESSED]).ToList();
            var bytesBilled = qrList.Select(x => x.NumData[Data.BYTES_BILLED]).ToList();

            var averageExecutionTime = executionTime.Average();
            var executionStdDev = executionTime.StdDev();
            var minExecutionTime = executionTime.Min();
            var maxExecutionTime = executionTime.Max();
            

            var avgBytesProcessed = bytesProcessed.Average();
            var bytesProcessesStdDev = bytesProcessed.StdDev();

            var avgBytesBilled = bytesBilled.Average();
            var bytesBilledStdDev = bytesBilled.StdDev();

            var numData = new Dictionary<Data, float>
            {
                { Data.AVG_EXECUTION_TIME, averageExecutionTime },
                { Data.EXECUTION_STD_DEV, executionStdDev },
                { Data.MIN_EXECUTION_TIME, minExecutionTime },
                { Data.MAX_EXECUTION_TIME, maxExecutionTime },
                { Data.AVG_BYTES_PROCESSED, avgBytesProcessed },
                { Data.BYTES_PROCESSED_STD_DEV, bytesProcessesStdDev },
                { Data.AVG_BYTES_BILLED, avgBytesBilled },
                { Data.BYTES_BILLED_STD_DEV, bytesBilledStdDev }
            };

            var strData = new Dictionary<Data, string>()
            {
                { Data.BI_MODE, ParseMultipleBiEngineResults(qrList.Select(x => x.StringData[Data.BI_MODE])) }
            };

            return new TableResult(numData, strData);
        }

        private string ParseMultipleBiEngineResults(IEnumerable<string> biEngineResults)
        {
            var dataList = biEngineResults.ToList(); // avoid multiple enumeration 
            var allBiEngineAreSame = dataList.Distinct().Count() == 1;
            return allBiEngineAreSame
                ? ParseBiEngine(dataList.First())
                : dataList.Aggregate((a, b) => $"{a}, {b}");
        }

        private string ParseBiEngine(string biEngineResult)
        {
            return biEngineResult switch
            {
                "NONE" => "N",
                "FULL" => "F",
                "PARTIAL" => "P",
                _ => "N"
            };
        }
    }
}