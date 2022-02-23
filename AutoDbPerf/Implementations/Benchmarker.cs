using System.Linq;
using AutoDbPerf.Interfaces;
using Microsoft.Extensions.Logging;

namespace AutoDbPerf.Implementations
{
    public class Benchmarker
    {
        private readonly IDirectoryScanner _directoryScanner;
        private readonly ILogger<Benchmarker> _logger;
        private readonly IQueryManager _queryManager;
        private readonly IQueryResultsAnalyser _queryResultsAnalyser;
        private readonly ITableOutput _tableOutput;

        public Benchmarker(
            ILoggerFactory loggerFactory,
            IQueryManager queryManager,
            ITableOutput tableOutput,
            IDirectoryScanner directoryScanner,
            IQueryResultsAnalyser queryResultsAnalyser
        )
        {
            _logger = loggerFactory.CreateLogger<Benchmarker>();
            _queryManager = queryManager;
            _tableOutput = tableOutput;
            _directoryScanner = directoryScanner;
            _queryResultsAnalyser = queryResultsAnalyser;
        }

        public string GetBenchmarks(string queryPath, int avgPrecision, int timeout)
        {
            _logger.LogInformation("Scanning directories");
            var queryInfo = _directoryScanner.ScanDirectories(queryPath).ToArray();
            var queryResults = _queryManager.GetQueryResults(queryInfo, avgPrecision, timeout);
            var tableData = _queryResultsAnalyser.GetTableData(queryResults);
            return _tableOutput.OutputResults(tableData);
        }
    }
}