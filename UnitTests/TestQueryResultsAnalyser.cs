using System;
using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Implementations;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using NSubstitute;
using NUnit.Framework;

namespace test_auto_db_perf
{
    [TestFixture]
    public class TestQueryResultsAnalyser
    {
        private class Context : IContext
        {
            private readonly bool _ignoreFirst;

            public Context(bool ignoreFirst)
            {
                _ignoreFirst = ignoreFirst;
            }

            public string GetEnv(ContextKey contextKey)
            {
                return contextKey switch
                {
                    ContextKey.IGNOREFIRST => _ignoreFirst.ToString().ToLower(),
                    _ => throw new ArgumentOutOfRangeException(nameof(contextKey), contextKey, null)
                };
            }
        }

        private Dictionary<Data, string> GetTestDictionary(string content)
        {
            return new Dictionary<Data, string>
            {
                { Data.BI_MODE, content }
            };
        }

        private IQueryResultAggregator _queryResultAggregator;

        [SetUp]
        public void Setup()
        {
            _queryResultAggregator = Substitute.For<IQueryResultAggregator>();
        }


        [Test]
        public void ProblemResults_AreIgnored_WhenMoreResultsAvailable()
        {
            var queryResults = new List<QueryResult>
            {
                new("scenario1", "query1", null, null),
                new("scenario1", "query1", null, null, true),
                new("scenario1", "query1", null, null),
            };

            var queryResultsAnalyser = new QueryResultsAnalyser(new Context(false), _queryResultAggregator);

            queryResultsAnalyser.GetTableData(queryResults);
            _queryResultAggregator.Received()
                .GetTableDataFrom(
                    Arg.Is<List<QueryResult>>(x => x.Any(qr => !qr.HasProblem))
                );
        }

        [Test]
        public void ProblemResult_IsReturned_WhenAllResultsHaveProblems()
        {
            var queryResults = new List<QueryResult>
            {
                new("scenario1", "query1", null, null, true),
                new("scenario1", "query1", null, null, true),
            };

            var queryResultsAnalyser = new QueryResultsAnalyser(new Context(false), _queryResultAggregator);

            var result = queryResultsAnalyser.GetTableData(queryResults);
            var tableResult = result.GetTableResult("scenario1", "query1");

            Assert.That(tableResult.HasProblem);
        }

        [Test]
        public void WithIgnoreFirst_WillIgnoreFirstSuccessfulResult()
        {
            var queryResults = new List<QueryResult>
            {
                new("scenario1", "query1", null, null, true),
                new("scenario1", "query1", null, GetTestDictionary("first")),
                new("scenario1", "query1", null, GetTestDictionary("second")),
            };

            var queryResultsAnalyser = new QueryResultsAnalyser(new Context(true), _queryResultAggregator);

            queryResultsAnalyser.GetTableData(queryResults);
            _queryResultAggregator.Received()
                .GetTableDataFrom(
                    Arg.Is<List<QueryResult>>(x => x.All(qr =>
                        qr.StringData.ContainsKey(Data.BI_MODE) &&
                        qr.StringData[Data.BI_MODE] == "second"))
                );
        }

        [Test]
        public void WithIgnoreFirstFalse_WillNotIgnoreFirstSuccessfulResult()
        {
            var queryResults = new List<QueryResult>
            {
                new("scenario1", "query1", null, null, true),
                new("scenario1", "query1", null, GetTestDictionary("first")),
                new("scenario1", "query1", null, GetTestDictionary("second")),
            };

            var queryResultsAnalyser = new QueryResultsAnalyser(new Context(false), _queryResultAggregator);

            queryResultsAnalyser.GetTableData(queryResults);
            _queryResultAggregator.Received()
                .GetTableDataFrom(
                    Arg.Is<List<QueryResult>>(x => x.Any(qr =>
                        qr.StringData.ContainsKey(Data.BI_MODE) &&
                        qr.StringData[Data.BI_MODE] == "first"))
                );
        }

        [Test]
        public void WithIgnoreFirst_WillReturnProblemResult_IfFirstSuccessfulResultIsOnlyResult()
        {
            var queryResults = new List<QueryResult>
            {
                new("scenario1", "query1", null, null, true),
                new("scenario1", "query1", null, GetTestDictionary("first"))
            };

            var queryResultsAnalyser = new QueryResultsAnalyser(new Context(true), _queryResultAggregator);


            var result = queryResultsAnalyser.GetTableData(queryResults);
            var tableResult = result.GetTableResult("scenario1", "query1");

            Assert.That(tableResult.HasProblem, Is.True);
        }

        [Test]
        public void WithIgnoreFirst_AndManyBadResults_WillAcceptSecondOfTwoSuccessfulResults()
        {
            var queryResults = new List<QueryResult>
            {
                new("scenario1", "query1", null, null, true),
                new("scenario1", "query1", null, null, true),
                new("scenario1", "query1", null, null, true),
                new("scenario1", "query1", null, GetTestDictionary("first")),
                new("scenario1", "query1", null, null, true),
                new("scenario1", "query1", null, GetTestDictionary("accepted")),
                new("scenario1", "query1", null, null, true),
                new("scenario1", "query1", null, GetTestDictionary("accepted")),
            };

            var queryResultsAnalyser = new QueryResultsAnalyser(new Context(true), _queryResultAggregator);

            queryResultsAnalyser.GetTableData(queryResults);
            _queryResultAggregator.Received()
                .GetTableDataFrom(
                    Arg.Is<List<QueryResult>>(x => x.All(qr =>
                        qr.StringData.ContainsKey(Data.BI_MODE) &&
                        qr.StringData[Data.BI_MODE] == "accepted"))
                );
        }
    }
}