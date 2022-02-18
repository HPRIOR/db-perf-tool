using System;
using System.Collections.Generic;

namespace AutoDbPerf.Records
{
    public record QueryData(float NumData, string StrData = null);
    public class QueryResult2
    {
        public QueryResult2(string scenario, string query,
            Dictionary<string, QueryData> data = null, bool hasProblem = false, string problem = "")
        {
            Data = data;
            HasProblem = hasProblem;
            Problem = problem;
            Query = query;
            Scenario = scenario;
            Time = DateTime.Now.TimeOfDay;
        }

        public readonly TimeSpan Time;
        public readonly Dictionary<string, QueryData> Data;
        public readonly bool HasProblem;
        public readonly string Problem;
        public readonly string Query;
        public readonly string Scenario;
    }
}