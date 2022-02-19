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

            var averageExecutionTime = qrList.Average(x => x.NumData["ExecutionTime"]);
            var executionStdDev = qrList.Select(x => x.NumData["ExecutionTime"]).StdDev();

            var avgBytesProcessed = qrList.Average(x => x.NumData["BytesProcessed"]);
            var bytesProcessesStdDev = qrList.Select(x => x.NumData["BytesProcessed"]).StdDev();

            var avgBytesBilled = qrList.Average(x => x.NumData["BytesBilled"]);
            var bytesBilledStdDev = qrList.Select(x => x.NumData["BytesBilled"]).StdDev();

            var numData = new Dictionary<string, float>
            {
                { "AvgExecutionTime", averageExecutionTime },
                { "ExecutionStdDev", executionStdDev },
                { "AvgBytesProcessed", avgBytesProcessed },
                { "BytesProcessed", bytesProcessesStdDev },
                { "AvgBytesBilled", avgBytesBilled },
                { "BytesBilledStdDev", bytesBilledStdDev }
            };

            var strData = new Dictionary<string, string>()
            {
                { "BiMode", ParseMultipleBiEngineResults(qrList.Select(x => x.StringData["BiMode"])) }
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