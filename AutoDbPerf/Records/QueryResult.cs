using System;

namespace AutoDbPerf.Records
{
    public record QueryResult
    {
        public QueryResult(float planningTime, float executionTime, string query, string scenario, string problem = "")
        {
            PlanningTime = planningTime;
            Query = query;
            Scenario = scenario;
            Problem = problem;
            ExecutionTime = executionTime;
            Time = DateTime.Now.TimeOfDay;
        }

        public float PlanningTime { get; }
        public float ExecutionTime { get; }
        public string Query { get; }
        public string Scenario { get; }
        public string Problem { get; }
        public TimeSpan Time { get; }
    }
}