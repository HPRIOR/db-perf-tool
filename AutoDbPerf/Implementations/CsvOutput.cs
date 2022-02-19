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
        private readonly ITableDataInterpreter _tableDataInterpreter;

        private readonly ILogger<CsvOutput> _logger;

        public CsvOutput(ILoggerFactory loggerFactory, ITableDataInterpreter tableDataInterpreter)
        {
            _tableDataInterpreter = tableDataInterpreter;
            _logger = loggerFactory.CreateLogger<CsvOutput>();
        }

        public string OutputResults(TableData tableData)
        {
            _logger.LogInformation("Creating csv");
            var sb = new StringBuilder();
            var columnRow = "scenarios," + tableData.Columns.Aggregate((a, b) => a + "," + b) + "\n";
            sb.Append(columnRow);

            tableData.Rows.Zip(GetDataFrom(tableData))
                .Select(x => (rowId: x.First, rowData: x.Second))
                .ToList()
                .ForEach(row =>
                    sb.Append(row.rowId + "," + row.rowData.Aggregate((a, b) => a + "," + b) + "\n"));
            return sb.ToString();
        }

        private IEnumerable<IEnumerable<string>> GetDataFrom(TableData tableData)
        {
            return tableData.Rows
                .Select(row => tableData.Columns.Select(column =>
                        tableData.RowColumnData.ContainsKey((column, row))
                            ? tableData.RowColumnData[(column, row)]
                            : new TableResult(null, null, true,  true))
                    .Select(tr => _tableDataInterpreter.Interpret(tr)));
        }

    }
}