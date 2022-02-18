using System;
using System.Runtime.CompilerServices;

namespace AutoDbPerf.Records
{
    
    /*
     * Tech specific fields should be included in a HashMap.
     * The rest can stay the same (Message: string, Problem: bool)
     */
    public record TableResult(
        float AvgPlanningTime,
        float AvgExecutionTime,
        float PlanningStdDev = 0,
        float ExecutionStdDev = 0,
        string Message = "",
        float GbProcessed = 0,
        string BiEngine = ""
        );
}