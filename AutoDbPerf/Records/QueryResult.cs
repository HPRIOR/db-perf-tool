using System;
using System.Collections.Generic;

namespace AutoDbPerf.Records
{
    public class QueryResult
    {
        public QueryResult(string scenario, string query,
            Dictionary<string, float> numData, Dictionary<string,string> stringData, bool hasProblem = false, string problem = "")
        {
            NumData = numData;
            StringData = stringData;
            HasProblem = hasProblem;
            Problem = problem;
            Query = query;
            Scenario = scenario;
            Time = DateTime.Now.TimeOfDay;
        }

        public readonly TimeSpan Time;
        public readonly Dictionary<string, float> NumData;
        public readonly Dictionary<string, string> StringData;
        public readonly bool HasProblem;
        public readonly string Problem;
        public readonly string Query;
        public readonly string Scenario;
    }
}