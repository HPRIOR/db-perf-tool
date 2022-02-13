using System;
using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations
{
    // TODO - create a results analyser for each IQueryResult interface (bq, ch, pg)
    // Returns specific table data e.g. BqTableData
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
                .ToDictionary(
                    x => (x.First().Scenario, x.First().Query),
                    queryResults =>
                    {
                        var filteredQueryResults = ApplyIgnoreFirstRule(queryResults).ToList();

                        if (ResultsAreAllBad(filteredQueryResults))
                        {
                            var message = GetErrorMessageFromResults(filteredQueryResults);
                            return new TableResult(0, 0, 0, 0, message);
                        }

                        var data = filteredQueryResults.Where(ResultHasData).ToList(); // will return min of 1 result

                        // one or more results must be available from here
                        var averagePlanningTime = GetAverage(TimeType.Planning, data);
                        var averageExecutionTime = GetAverage(TimeType.Execution, data);
                        var averageBytesProcessed = GetAverageBytesProcessed(data);

                        if (data.Count == 1) // std deviation not possible - default to 0
                        {   
                            var biEngine = ParseBiEngine(data[0].BiEngine);
                            return new TableResult(averagePlanningTime, averageExecutionTime, 0, 0, "",
                                averageBytesProcessed, biEngine);
                        }


                        var planningStdDev = GetStdDev(TimeType.Planning, filteredQueryResults);
                        var executionStdDev = GetStdDev(TimeType.Execution, filteredQueryResults);
                        var multiBiEngine = ParseMultipleBiEngineResults(data.Select(x => x.BiEngine));

                        return new TableResult(
                            averagePlanningTime,
                            averageExecutionTime,
                            planningStdDev,
                            executionStdDev,
                            "",
                            averageBytesProcessed,
                            multiBiEngine
                        );
                    });
        }

        private float GetAverageBytesProcessed(List<QueryResult> data) => data.Average(d => d.BytesProcessed);


        private IEnumerable<QueryResult> ApplyIgnoreFirstRule(IEnumerable<QueryResult> queryResults)
        {
            return _ctx.GetEnv(ContextKey.IGNOREFIRST) == "true"
                ? queryResults
                    .OrderBy(qr => qr.Time)
                    .AllAfterFirstSuccessful()
                    .ToList()
                : queryResults;
        }

        private float GetAverage(TimeType timeType, IEnumerable<QueryResult> queryResults) => queryResults
            .Average(qr => OfTimeType(timeType, qr));

        private float GetStdDev(TimeType timeType, IEnumerable<QueryResult> queryResults)
        {
            var data = queryResults.Where(ResultHasData).Select(qr => OfTimeType(timeType, qr))
                .ToList(); // will be greater than 
            var avg = data.Average();
            return (float)Math.Round(
                Math.Sqrt(
                    data.Select(x => Math.Pow(x - avg, 2)).Average()
                ), 2);
        }

        private float OfTimeType(TimeType timeType, QueryResult qr) => timeType switch
        {
            TimeType.Execution => qr.ExecutionTime,
            TimeType.Planning => qr.PlanningTime,
            _ => throw new ArgumentOutOfRangeException(nameof(timeType), timeType, null)
        };

        private bool ResultsAreAllBad(IEnumerable<QueryResult> results)
        {
            return results.All(result => result.Problem.Length > 0);
        }

        private bool ResultHasData(QueryResult result)
        {
            return result.Problem.Length == 0;
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

        private string ParseMultipleBiEngineResults(IEnumerable<string> biEngineResults)
        {
            var dataList = biEngineResults.ToList(); // avoid multiple enumeration 
            var allBiEngineAreSame = dataList.Distinct().Count() == 1;
            return allBiEngineAreSame 
                ? ParseBiEngine(dataList.First()) 
                : dataList.Aggregate((a, b) => $"{a}, {b}");
        }

        private string ParseBiEngine(string biEngineResult)
        {
            return biEngineResult switch
            {
                "NONE" => "N",
                "FULL" => "F",
                "PARTIAL" => "P",
                _ => "N"
            };
        }
    }
}