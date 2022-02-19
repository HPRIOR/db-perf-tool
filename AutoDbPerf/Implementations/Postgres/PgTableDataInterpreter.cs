using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;

namespace AutoDbPerf.Implementations.Postgres
{
    public class PgTableDataInterpreter : ITableDataInterpreter
    {
        public string Interpret(TableResult tr)
        {
            if (tr.HasProblem)
                return "Error - see logs";
            return
                $"Planning: {tr.NumericData["AvgPlanningTime"]} " +
                $"SD: {tr.NumericData["PlanningStdDev"]} " +
                $"Execution: {tr.NumericData["AvgExecutionTime"]} " +
                $"SD: {tr.NumericData["ExecutionStdDev"]} " +
                $"Total: {tr.NumericData["AvgPlanningTime"] + tr.NumericData["AvgExecutionTime"]}";
        }
    }
}