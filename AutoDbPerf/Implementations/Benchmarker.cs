using AutoDbPerf.Interfaces;

namespace AutoDbPerf.Implementations
{
    public class Benchmarker
    {
        private readonly IQueryManager _queryManager;
        private readonly IResultAnalyser _resultAnalyser;
        private readonly ITableOutput _tableOutput;

        public Benchmarker(
            IQueryManager queryManager,
            IResultAnalyser resultAnalyser,
            ITableOutput tableOutput
        )
        {
            _queryManager = queryManager;
            _resultAnalyser = resultAnalyser;
            _tableOutput = tableOutput;
        }

        public string GetBenchmarks(string queryPath, int avgPrecision, int timeout)
        {
            var results =
                _queryManager.GetQueryResult(queryPath, avgPrecision,
                    timeout);
            var tableData = _resultAnalyser.AnalyseResults(results);
            return _tableOutput.OutputResults(tableData);
        }
    }
}