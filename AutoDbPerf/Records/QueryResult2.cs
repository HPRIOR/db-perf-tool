using System;
using System.Collections.Generic;

namespace AutoDbPerf.Records
{
    public class QueryResult2
    {
        public QueryResult2(string scenario, string query, bool hasProblem, string problem, string queryResult,
            Dictionary<string, float> data = null)
        {
            Data = data;
            HasProblem = hasProblem;
            Problem = problem;
            Query = query;
            QueryResult = queryResult;
            Scenario = scenario;
            Time = DateTime.Now.TimeOfDay;
        }

        public readonly TimeSpan Time;
        public readonly Dictionary<string, float> Data;
        public readonly bool HasProblem;
        public readonly string Problem;
        public readonly string Query;
        public readonly string QueryResult;
        public readonly string Scenario;
    }
}