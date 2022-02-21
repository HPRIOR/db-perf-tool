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
        private readonly IContext _context;
        private readonly IQueryExecutor _queryExecutor;

        public QueryManager(
            IContext context,
            IQueryExecutor queryExecutor
        )
        {
            _context = context;
            _queryExecutor = queryExecutor;
        }

        private record ScenarioQuery(string Scenario, string Query);

        public IEnumerable<QueryResult> GetQueryResults(IEnumerable<QueryInfo> queryInfo, int avgPrecision,
            int timeout = 5000)
        {
            if (avgPrecision <= 0)
                throw new ArgumentException("Average precision must be greater than 0");

            var multipliedScenarioQueries =
                queryInfo
                    .SelectMany(scenarioQueryPath => Enumerable.Range(0, avgPrecision)
                        .SelectMany(avgGroup => scenarioQueryPath
                            .Queries
                            .OrderBy(x => x)
                            .Select(query => (new ScenarioQuery(scenarioQueryPath.Scenario, query), avgGroup))));

            return GetOrderedQueries(multipliedScenarioQueries)
                .Select(sqp => _queryExecutor.ExecuteQuery(sqp.Query, sqp.Scenario, timeout));
        }


        private IEnumerable<ScenarioQuery> GetOrderedQueries(IEnumerable<(ScenarioQuery, int)> scenarioQueries)
        {
            var orderContext = _context.GetEnv(ContextKey.ORDER);
            return orderContext switch
            {
                "rr" => scenarioQueries.GroupBy(tuple => tuple.Item2).SelectMany(group => group.Select(x => x.Item1)),
                "seq" => scenarioQueries.OrderBy(sq => sq.Item1.Scenario).ThenBy(sq => sq.Item1.Query).Select(x => x.Item1),
                _ => scenarioQueries.OrderBy(sq => sq.Item1.Scenario).Select(x => x.Item1)
            };
        }
    }
}