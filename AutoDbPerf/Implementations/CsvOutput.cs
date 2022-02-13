using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using Microsoft.Extensions.Logging;

namespace AutoDbPerf.Implementations
{
    public class CsvOutput : ITableOutput
    {
        // TODO - create db specific implementation to handle db specific TableData
        private readonly ILogger<CsvOutput> _logger;

        public CsvOutput(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CsvOutput>();
        }

        public string OutputResults(TableData tableData)
        {
            _logger.LogInformation("Creating csv");
            var sb = new StringBuilder();
            var columnRow = "scenarios," + tableData.Columns.Aggregate((a, b) => a + "," + b) + "\n";
            sb.Append(columnRow);

            tableData.Rows.Zip(getDataFrom(tableData)).ToList().ForEach(row =>
                sb.Append(row.First + "," + row.Second.Aggregate((a, b) => a + "," + b) + "\n"));
            return sb.ToString();
        }

        private IEnumerable<IEnumerable<string>> getDataFrom(TableData tableData)
        {
            return tableData.Rows
                .Select(row => tableData.Columns.Select(column =>
                        tableData.RowColumnData.ContainsKey((column, row))
                            ? tableData.RowColumnData[(column, row)]
                            : new TableResult(0, 0))
                    .Select(tr =>
                        {
                            // refactor this is getting messy
                            if (tr.Message.Length > 0)
                                return "Error - see logs";
                            if (tr.AvgPlanningTime == 0 && tr.AvgExecutionTime != 0)
                            {
                                var avgBytesProcessed = tr.GbProcessed > 0 ? $" Gb: {tr.GbProcessed}" : "";
                                return $"Execution: {tr.AvgExecutionTime}ms SD: {tr.ExecutionStdDev}{avgBytesProcessed} ({tr.BiEngine})";
                            }

                            if (tr.AvgExecutionTime != 0 && tr.AvgPlanningTime != 0)
                                return
                                    $"Planning: {tr.AvgPlanningTime} SD: {tr.PlanningStdDev} Execution: {tr.AvgExecutionTime} SD: {tr.ExecutionStdDev} Total: {tr.AvgPlanningTime + tr.AvgExecutionTime}";
                            return "N/A";
                        }
                    ));
        }
    }
}