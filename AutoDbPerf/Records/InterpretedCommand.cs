namespace AutoDbPerf.Records
{
    public record InterpretedCommand(bool IsError, float ExecutionTime = 0, float PlanningTime = 0,
        string ErrorMessage = "");
}