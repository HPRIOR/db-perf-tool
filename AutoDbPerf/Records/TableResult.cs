using System;
using System.Runtime.CompilerServices;

namespace AutoDbPerf.Records
{
    public record TableResult(
        float AvgPlanningTime,
        float AvgExecutionTime,
        float PlanningStdDev = 0,
        float ExecutionStdDev = 0,
        string Message = ""
        );
}