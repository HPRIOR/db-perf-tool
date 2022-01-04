namespace AutoDbPerf.Records
{
    public record InterpretedCommand
    {
        public InterpretedCommand(bool isError, float executionTime = 0, float planningTime = 0,
            string errorMessage = "")
        {
            IsError = isError;
            ExecutionTime = executionTime;
            PlanningTime = planningTime;
            ErrorMessage = errorMessage;
        }

        public bool IsError { get; }
        public float ExecutionTime { get; }
        public float PlanningTime { get; }
        public string ErrorMessage { get; }
    }
}