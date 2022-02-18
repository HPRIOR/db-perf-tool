using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using Microsoft.Extensions.Logging;

namespace AutoDbPerf.Implementations
{
    public class QueryManager : IQueryManager
    {
        private readonly IDirectoryScanner _directoryScanner;
        private readonly ILogger<QueryManager> _logger;
        private readonly IContext _context;
        private readonly IQueryExecutor _queryExecutor;

        public QueryManager(
            ILoggerFactory loggerFactory,
            IContext context,
            IQueryExecutor queryExecutor,
            IDirectoryScanner directoryScanner
        )
        {
            _logger = loggerFactory.CreateLogger<QueryManager>();
            _context = context;
            _queryExecutor = queryExecutor;
            _directoryScanner = directoryScanner;
        }

        private record ScenarioQuery(string Scenario, string Query);

        public IEnumerable<QueryResult2> GetQueryResult(string queryPath, int avgPrecision, int timeout = 5000)
        {
            if (avgPrecision <= 0)
                throw new ArgumentException("Average precision must be greater than 0");

            // lift me up and pass in results - separate I/O 
            _logger.LogInformation("Scanning directories");
            var scenarioQueryPaths = _directoryScanner.ScanDirectories(queryPath).ToArray();

            var multipliedScenarioQueries =
                scenarioQueryPaths.SelectMany(scenarioQueryPath => Enumerable.Range(0, avgPrecision).SelectMany(_ =>
                    scenarioQueryPath.Queries.OrderBy(x => x).Select(query => new ScenarioQuery
                        (scenarioQueryPath.Scenario, query))));

            return GetOrderedQueries(multipliedScenarioQueries).Select(sqp =>
                _queryExecutor.ExecuteQuery(sqp.Query, sqp.Scenario, timeout));
        }

        private IEnumerable<ScenarioQuery> GetOrderedQueries(IEnumerable<ScenarioQuery> scenarioQueries)
        {
            var orderContext = _context.GetEnv(ContextKey.ORDER);
            return orderContext switch
            {
                "rr" => scenarioQueries.OrderBy(sq => sq.Scenario),
                "seq" => scenarioQueries.OrderBy(sq => sq.Scenario).ThenBy(sq => sq.Query),
                _ => scenarioQueries.OrderBy(sq => sq.Scenario)
            };
            
        }
    }
}