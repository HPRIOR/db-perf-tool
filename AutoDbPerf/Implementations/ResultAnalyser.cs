using System;
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

        private enum TimeType
        {
            Execution,
            Planning
        }

        public ResultAnalyser(IContext ctx)
        {
            _ctx = ctx;
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
                .ToDictionary(x => (x.First().Scenario, x.First().Query), queryResults =>
                {
                    var filteredQueryResults = _ctx.GetEnv(ContextKey.IGNOREFIRST) == "true"
                        ? queryResults.OrderBy(qr => qr.Time).AllButFirst().ToList()
                        : queryResults;
                    
                    if (ResultsAreAllBad(filteredQueryResults))
                    {
                        var message = GetMessageFromResults(filteredQueryResults);
                        return new TableResult(0, 0, message);
                    }

                    var averagePlanningTime = GetAverage(TimeType.Planning, filteredQueryResults);
                    var averageExecutionTime = GetAverage(TimeType.Execution, filteredQueryResults);
                    return new TableResult(averagePlanningTime, averageExecutionTime);
                });
        }

        private float GetAverage(TimeType timeType, IEnumerable<QueryResult> queryResults) => queryResults
            .Where(ResultHasData).Average(
                x => timeType switch
                {
                    TimeType.Execution => x.ExecutionTime,
                    TimeType.Planning => x.PlanningTime,
                    _ => throw new ArgumentOutOfRangeException(nameof(timeType), timeType, null)
                }
            );

        private bool ResultsAreAllBad(IEnumerable<QueryResult> results)
        {
            return results.All(result => result.Problem.Length > 0);
        }

        private bool ResultHasData(QueryResult result)
        {
            return result.Problem.Length == 0;
        }

        private string GetMessageFromResults(IEnumerable<QueryResult> results)
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