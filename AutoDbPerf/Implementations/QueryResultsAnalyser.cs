using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations
{
    using TableResults = Dictionary<(string scenario, string query), TableResult>;

    public class QueryResultsAnalyser : IQueryResultsAnalyser
    {
        private readonly IContext _ctx;
        private readonly IQueryResultAggregator _queryResultAggregator;

        public QueryResultsAnalyser(IContext ctx, IQueryResultAggregator queryResultAggregator)
        {
            _ctx = ctx;
            _queryResultAggregator = queryResultAggregator;
        }

        public TableData GetTableData(IEnumerable<QueryResult> queryResults)
        {
            return new TableData(GetCellResults(queryResults));
        }

        private TableResults GetCellResults(
            IEnumerable<QueryResult> results)
        {
            return results
                .GroupBy(qr => new { qr.Scenario, qr.Query })
                .Select(group => group.ToList())
                .ToDictionary(
                    x => (x.First().Scenario, x.First().Query),
                    queryResults =>
                    {
                        var filteredQueryResults = ApplyIgnoreFirstRule(queryResults).ToList();

                        if (AllResultsHaveProblems(filteredQueryResults))
                            return new TableResult(null, null, true);

                        var data = filteredQueryResults.Where(ResultHasData).ToList(); // will return min of 1 result
                        return _queryResultAggregator.GetTableDataFrom(data);
                    });
        }

        private IEnumerable<QueryResult> ApplyIgnoreFirstRule(IEnumerable<QueryResult> queryResults)
        {
            return _ctx.GetEnv(ContextKey.IGNOREFIRST) == "true"
                ? queryResults
                    .OrderBy(qr => qr.Time)
                    .AllAfterFirstSuccessful()
                    .ToList()
                : queryResults;
        }

        private bool AllResultsHaveProblems(IEnumerable<QueryResult> results)
        {
            return results.All(result => result.HasProblem);
        }

        private bool ResultHasData(QueryResult result)
        {
            return !result.HasProblem;
        }
    }
}