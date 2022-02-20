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
        private readonly IQueryResultInterpreter _queryResultInterpreter;
        private readonly ILogger<QueryManager> _logger;
        private readonly IContext _context;
        private readonly IQueryExecutor _queryExecutor;

        public QueryManager(
            ILoggerFactory loggerFactory,
            IContext context,
            IQueryExecutor queryExecutor,
            IDirectoryScanner directoryScanner,
            IQueryResultInterpreter queryResultInterpreter
        )
        {
            _logger = loggerFactory.CreateLogger<QueryManager>();
            _context = context;
            _queryExecutor = queryExecutor;
            _directoryScanner = directoryScanner;
            _queryResultInterpreter = queryResultInterpreter;
        }

        private record ScenarioQuery(string Scenario, string Query);

        public IEnumerable<QueryResult> GetQueryResults(IEnumerable<QueryInfo> queryInfo, int avgPrecision,
            int timeout = 5000)
        {
            if (avgPrecision <= 0)
                throw new ArgumentException("Average precision must be greater than 0");

            var multipliedScenarioQueries =
                queryInfo.SelectMany(scenarioQueryPath => Enumerable.Range(0, avgPrecision).SelectMany(_ =>
                    scenarioQueryPath.Queries.OrderBy(x => x).Select(query => new ScenarioQuery
                        (scenarioQueryPath.Scenario, query))));

            return multipliedScenarioQueries.Select(sqp =>
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