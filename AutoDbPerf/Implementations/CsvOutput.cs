using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;
using Microsoft.Extensions.Logging;

namespace AutoDbPerf.Implementations
{
    public class CsvOutput : ITableOutput
    {
        private readonly ILogger<CsvOutput> _logger;

        public CsvOutput(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CsvOutput>();
        }

        public string OutputResults(TableData tableData)
        {
            _logger.LogInformation("Creating csv");
            var numberOfScenarios = tableData.ScenarioColumns.Count();
            var numberOfDataPoints = tableData.OrderedDataColumns.Count;
            var sb = new StringBuilder();
            var scenarioColumnsRow = "scenarios," + tableData.ScenarioColumns.Aggregate((a, b) => a + ",".MultiplyBy(numberOfDataPoints) + b) + "\n";
            sb.Append(scenarioColumnsRow);


            var orderedDataColumnsRow =
                $",{tableData.OrderedDataColumns.Aggregate((a, b) => $"{a},{b}").MultiplyBy(numberOfScenarios, ",")}\n"; // multiplied by number of scenarios
            sb.Append(orderedDataColumnsRow);

            tableData.Rows.Zip(GetDataFrom(tableData, numberOfDataPoints))
                .Select(x => (rowId: x.First, rowData: x.Second))
                .ToList()
                .ForEach(row =>
                    sb.Append($"{row.rowId},{row.rowData.Aggregate((a, b) => a + "," + b)}\n"));

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
                .Where(od => (tr.NumericData?.ContainsKey(od) ?? false) || (tr.StringData?.ContainsKey(od) ?? false))
                .Select(d =>
                    tr.NumericData.ContainsKey(d)
                        ? tr.NumericData[d].ToString(CultureInfo.InvariantCulture)
                        : tr.StringData[d])
                .Aggregate((a, b) => $"{a},{b}");
        }
    }
}