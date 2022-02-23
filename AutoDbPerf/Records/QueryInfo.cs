using System.Collections.Generic;

namespace AutoDbPerf.Records
{
    public record QueryInfo(string Scenario, IEnumerable<string> Queries);
}