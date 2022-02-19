using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;

namespace AutoDbPerf.Implementations
{
    public class GeneralTableDataInterpreter : ITableDataInterpreter
    {
        public string Interpret(TableResult tr)
        {
            if (tr.HasProblem)
                return "Error - see logs";
            return
                $"Execution: {tr.NumericData["AvgExecutionTime"]} SD: {tr.NumericData["ExecutionStdDev"]}";
        }
    }
}