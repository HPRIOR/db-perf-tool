using System;
using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Implementations;
using AutoDbPerf.Utils;


namespace AutoDbPerf.Records
{
    using TableResults = Dictionary<(string scenario, string query), TableResult>;
    public record TableData
    {
        // try to remove queryResults from constructor
        public TableData(
            TableResults tableResults
        )
        {
            ScenarioColumns = GetScenarioColumns(tableResults);
            Rows = GetRows(tableResults);
            OrderedDataColumns = GetOrderedDataColumns(tableResults);
            _tableResults = tableResults;
        }

        private readonly TableResults _tableResults;

        public IEnumerable<string> ScenarioColumns { get; }
        public IEnumerable<string> Rows { get; }
        public List<string> OrderedDataColumns { get; }

        public TableResult GetTableResult(string scenario, string query)
        {
            return _tableResults[(scenario, query)];
        }

        public bool HasDataFor(string scenario, string query)
        {
            return _tableResults.ContainsKey((scenario, query));
        }

        private static TableResult? GetFirstTableResultWithData(
            TableResults scenarioQueryData)
        {
            if (!scenarioQueryData.Any(kv => kv.Value.DataNum != null || kv.Value.StringData != null))
                return null;

            return scenarioQueryData
                .First(kv => kv.Value.DataNum != null || kv.Value.StringData != null).Value;
        }

        private static  List<string> GetOrderedDataColumns(
            TableResults tableResults)
        {
            var firstTableResultWithData = GetFirstTableResultWithData(tableResults);
            var numericData = firstTableResultWithData?.NumericData?.Keys.ToList() ?? new List<Data>();
            var stringData = firstTableResultWithData?.StringData?.Keys.ToList() ?? new List<Data>();
            return GetOrderedEnums(numericData.Concat(stringData)).Select(x => x.AsString()).ToList();
        }

        private static List<Data> GetOrderedEnums(IEnumerable<Data> data)
        {
            return data.OrderBy(d =>
                {
                    var index = DataUtils.GetFixedOrderedData().IndexOf(d);
                    return index == -1 ? int.MaxValue : index;
                }
            ).ToList();
        }
        private static IEnumerable<string> GetScenarioColumns(TableResults tableResults)
        {
            return tableResults.Keys.Select(x => x.scenario).ToHashSet();
        }

        private static IEnumerable<string> GetRows(TableResults tableResults)
        {
            return tableResults.Keys.Select(x => x.query).ToHashSet();
        }
    };
}