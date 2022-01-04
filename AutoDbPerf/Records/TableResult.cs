namespace AutoDbPerf.Records
{
    public record TableResult
    {
        public TableResult(float avgPlanningTime, float avgExecutionTime, string message = "")
        {
            AvgPlanningTime = avgPlanningTime;
            AvgExecutionTime = avgExecutionTime;
            Message = message;
        }

        public float AvgPlanningTime { get; }
        public float AvgExecutionTime { get; }
        public string Message { get; }
    }
}