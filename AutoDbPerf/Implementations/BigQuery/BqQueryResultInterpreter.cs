using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations.BigQuery
{
    public class BqQueryResultInterpreter : IQueryResultInterpreter
    {
        public TableResult GetTableDataFrom(IEnumerable<QueryResult> queryResult)
        {
            var qrList = queryResult.ToList();

            var averageExecutionTime = qrList.Average(x => x.NumData[Data.EXECUTION_TIME]);
            var executionStdDev = qrList.Select(x => x.NumData[Data.EXECUTION_TIME]).StdDev();

            var avgBytesProcessed = qrList.Average(x => x.NumData[Data.BYTES_PROCESSED]);
            var bytesProcessesStdDev = qrList.Select(x => x.NumData[Data.BYTES_PROCESSED]).StdDev();

            var avgBytesBilled = qrList.Average(x => x.NumData[Data.BYTES_BILLED]);
            var bytesBilledStdDev = qrList.Select(x => x.NumData[Data.BYTES_BILLED]).StdDev();

            var numData = new Dictionary<Data, float>
            {
                { Data.AVG_EXECUTION_TIME, averageExecutionTime },
                { Data.EXECUTION_STD_DEV, executionStdDev },
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