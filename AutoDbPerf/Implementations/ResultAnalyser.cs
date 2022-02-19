using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations
{
    public class ResultAnalyser : IResultAnalyser
    {
        private readonly IContext _ctx;
        private readonly IQueryResultInterpreter _queryResultInterpreter;

        public ResultAnalyser(IContext ctx, IQueryResultInterpreter queryResultInterpreter)
        {
            _ctx = ctx;
            _queryResultInterpreter = queryResultInterpreter;
        }

        public TableData AnalyseResults(IEnumerable<QueryResult> results)
        {
            var resultArray = results.ToArray();
            var columns = GetColumns(resultArray);
            var rows = GetRows(resultArray);
            var dataDictionary = GetCellResults(resultArray);

            return new TableData(columns, rows, dataDictionary);
        }

        private Dictionary<(string, string), TableResult> GetCellResults(IEnumerable<QueryResult> results)
        {
            return results
                .GroupBy(qr => new { qr.Scenario, qr.Query })
                .Select(group => group.ToList())
                .ToDictionary(
                    x => (x.First().Scenario, x.First().Query),
                    queryResults =>
                    {
                        var filteredQueryResults = ApplyIgnoreFirstRule(queryResults).ToList();

                        if (ResultsAreAllBad(filteredQueryResults))
                        {
                            var message = GetErrorMessageFromResults(filteredQueryResults);
                            return new TableResult(null, null, true, message);
                        }


                        var data = filteredQueryResults.Where(ResultHasData).ToList(); // will return min of 1 result
                        return _queryResultInterpreter.GetTableDataFrom(data);
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

        private bool ResultsAreAllBad(IEnumerable<QueryResult> results)
        {
            return results.All(result => result.HasProblem);
        }

        private bool ResultHasData(QueryResult result)
        {
            return !result.HasProblem;
        }

        private string GetErrorMessageFromResults(IEnumerable<QueryResult> results)
        {
            return results
                .Select(r => r.Problem)
                .ToHashSet()
                .FlattenToCommaList();
        }


        private IEnumerable<string> GetColumns(IEnumerable<QueryResult> result)
        {
            return result.Select(r => r.Scenario).ToHashSet();
        }

        private IEnumerable<string> GetRows(IEnumerable<QueryResult> result)
        {
            return result.Select(r => r.Query).ToHashSet();
        }
    }
}