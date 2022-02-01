namespace AutoDbPerf.Records
{
    public record TableResult(float AvgPlanningTime, float AvgExecutionTime, string Message = "");
}