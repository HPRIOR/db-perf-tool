using System;

namespace AutoDbPerf.Records
{
    // TODO - Abstract query result into and interface with string -> string map to retrieve data
    public record QueryResult
    {
        /*
         * Tech specific fields should be included in a HashMap (planningTime, executionTime, bytesProcessed, biEngine).
         * The rest can stay the same 
         */
        public QueryResult(float planningTime, float executionTime, string query, string scenario, string problem = "", float bytesProcessed = 0, string biEngine = "")
        {
            BytesProcessed = bytesProcessed;
            PlanningTime = planningTime;
            Query = query;
            Scenario = scenario;
            Problem = problem;
            ExecutionTime = executionTime;
            Time = DateTime.Now.TimeOfDay;
            BiEngine = biEngine;
        }

        public string BiEngine { get;}

        public readonly float BytesProcessed;
        public float PlanningTime { get; }
        public float ExecutionTime { get; }
        public string Query { get; }
        public string Scenario { get; }
        public string Problem { get; }
        public TimeSpan Time { get; }
    }
}