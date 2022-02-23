using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;
using Google.Apis.Bigquery.v2.Data;
using Microsoft.Extensions.Logging;

namespace AutoDbPerf.Implementations
{
    public class CsvOutput : ITableOutput
    {
        private readonly IColumnOrderer _columnOrderer;
        private readonly ILogger<CsvOutput> _logger;

        public CsvOutput(ILoggerFactory loggerFactory, IColumnOrderer columnOrderer)
        {
            _columnOrderer = columnOrderer;
            _logger = loggerFactory.CreateLogger<CsvOutput>();
        }

        public string OutputResults(TableData tableData)
        {
            _logger.LogInformation("Creating csv");
            var initOrderedDataColumns = _columnOrderer.GetOrderedColumns(tableData);
            var orderedDataColumns = initOrderedDataColumns.Count == 0
                ? new List<string> { "Error" }
                : initOrderedDataColumns;

            var numberOfDataPoints = orderedDataColumns.Count == 0 ? 1 : orderedDataColumns.Count;
            var numberOfScenarios = tableData.ScenarioColumns.Count();


            var sb = new StringBuilder();
            var scenarioColumnsRow = "scenarios," +
                                     tableData.ScenarioColumns.Aggregate((a, b) =>
                                         a + ",".MultiplyBy(numberOfDataPoints) + b) + "\n";
            sb.Append(scenarioColumnsRow);


            var orderedDataColumnsRow =
                $",{orderedDataColumns.Aggregate((a, b) => $"{a},{b}").MultiplyBy(numberOfScenarios, ",")}\n"; // multiplied by number of scenarios
            sb.Append(orderedDataColumnsRow);

            var rows = tableData.Rows.Zip(GetDataFrom(tableData, numberOfDataPoints))
                .Select(x => (rowId: x.First, rowData: x.Second))
                .Select(row => $"{row.rowId},{row.rowData.Aggregate((a, b) => a + "," + b)}\n")
                .OrderBy(x => x)
                .Aggregate((a, b) => $"{a}{b}");


            sb.Append(rows);


            return sb.ToString();
        }

        private IEnumerable<IEnumerable<string>> GetDataFrom(TableData tableData, int numberOfDataPoints)
        {
            return tableData.Rows
                .Select(row => tableData.ScenarioColumns.Select(column =>
                        tableData.HasDataFor(column, row)
                            ? tableData.GetTableResult(column, row)
                            : new TableResult(null, null, true, true))
                    .Select(tr => InterpretTableResult(tr, numberOfDataPoints)));
        }

        private string InterpretTableResult(TableResult tr, int numberOfDataPoints)
        {
            if (tr.IsEmpty)
                return "N/A".MultiplyBy(numberOfDataPoints, ",");

            if (tr.HasProblem)
                return "Error - see logs".MultiplyBy(numberOfDataPoints, ",");

            var orderedData = DataUtils.GetFixedOrderedData();
            return orderedData
                .Where(d => (tr.NumericData?.ContainsKey(d) ?? false) || (tr.StringData?.ContainsKey(d) ?? false))
                .Select(d =>
                    tr.NumericData.ContainsKey(d)
                        ? tr.NumericData[d].ToString(CultureInfo.InvariantCulture)
                        : tr.StringData[d])
                .Aggregate((a, b) => $"{a},{b}");
        }
    }
}