using System.Collections.Generic;

namespace AutoDbPerf.Records
{
    public record ScenarioQueryPath(string Scenario, IEnumerable<string> Queries);
}