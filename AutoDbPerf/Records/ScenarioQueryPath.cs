using System.Collections.Generic;

namespace AutoDbPerf.Records
{
    public record ScenarioQueryPath
    {
        public ScenarioQueryPath(string scenario, IEnumerable<string> queries)
        {
            Scenario = scenario;
            Queries = queries;
        }

        public string Scenario { get; }
        public IEnumerable<string> Queries { get; }
    }
}