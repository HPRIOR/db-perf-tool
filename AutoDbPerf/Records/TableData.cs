using System.Collections.Generic;
using System.Linq;

namespace AutoDbPerf.Records
{
    using TableResults = Dictionary<(string scenario, string query), TableResult>;

    public record TableData
    {
        private readonly TableResults _tableResults;

        // try to remove queryResults from constructor
        public TableData(
            TableResults tableResults
        )
        {
            ScenarioColumns = GetScenarioColumns(tableResults);
            Rows = GetRows(tableResults);
            _tableResults = tableResults;
        }

        public IEnumerable<string> ScenarioColumns { get; }
        public IEnumerable<string> Rows { get; }

        public TableResult GetTableResult(string scenario, string query) => _tableResults[(scenario, query)];

        public bool HasDataFor(string scenario, string query) => _tableResults.ContainsKey((scenario, query));

        private static IEnumerable<string> GetScenarioColumns(TableResults tableResults)
        {
            return tableResults.Keys.Select(x => x.scenario).ToHashSet();
        }

        private static IEnumerable<string> GetRows(TableResults tableResults)
        {
            return tableResults.Keys.Select(x => x.query).ToHashSet();
        }
    }
}