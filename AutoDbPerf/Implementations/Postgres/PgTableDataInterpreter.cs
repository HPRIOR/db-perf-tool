using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;

namespace AutoDbPerf.Implementations.Postgres
{
    public class PgTableDataInterpreter : ITableDataInterpreter
    {
        public string Interpret(TableResult tr)
        {
            if (tr.IsEmpty)
                return "N/A";
            if (tr.HasProblem)
                return "Error - see logs";
            return
                $"Planning: {tr.NumericData[Data.AVG_PLANNING_TIME]} " +
                $"SD: {tr.NumericData[Data.PLANNING_STD_DEV]} " +
                $"Execution: {tr.NumericData[Data.AVG_EXECUTION_TIME]} " +
                $"SD: {tr.NumericData[Data.EXECUTION_STD_DEV]} " +
                $"Total: {tr.NumericData[Data.AVG_PLANNING_TIME] + tr.NumericData[Data.AVG_EXECUTION_TIME]}";
        }
    }
}