using System.Collections.Generic;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;

namespace AutoDbPerf.Implementations.BigQuery
{
    public class BqTableDataInterpreter : ITableDataInterpreter
    {
        public string Interpret(TableResult tr)
        {
            if (tr.IsEmpty)
                return "N/A";
            if (tr.HasProblem)
                return "Error - see logs";
            return
                $"Execution: {tr.NumericData["AvgExecutionTime"]}ms " +
                $"SD: {tr.NumericData["ExecutionStdDev"]}{tr.NumericData["AvgBytesProcessed"]} " +
                $"({tr.StringData["BiMode"]})";
        }
    }
}