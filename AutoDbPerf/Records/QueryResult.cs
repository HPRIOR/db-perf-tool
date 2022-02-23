using System;
using System.Collections.Generic;
using AutoDbPerf.Implementations;

namespace AutoDbPerf.Records
{
    public class QueryResult
    {
        public readonly bool HasProblem;
        public readonly Dictionary<Data, float> NumData;
        public readonly string Problem;
        public readonly string Query;
        public readonly string Scenario;
        public readonly Dictionary<Data, string> StringData;

        public readonly TimeSpan Time;

        public QueryResult(string scenario, string query,
            Dictionary<Data, float> numData, Dictionary<Data, string> stringData, bool hasProblem = false,
            string problem = "")
        {
            NumData = numData;
            StringData = stringData;
            HasProblem = hasProblem;
            Problem = problem;
            Query = query;
            Scenario = scenario;
            Time = DateTime.Now.TimeOfDay;
        }
    }
}