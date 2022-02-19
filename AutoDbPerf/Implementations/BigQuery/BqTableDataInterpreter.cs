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
                $"Execution: {tr.NumericData[Data.AVG_EXECUTION_TIME]}ms " +
                $"SD: {tr.NumericData[Data.EXECUTION_STD_DEV]} " +
                $"BytesProcessed: {tr.NumericData[Data.AVG_BYTES_BILLED]} " +
                $"({tr.StringData[Data.BI_MODE]})";
        }
    }
}