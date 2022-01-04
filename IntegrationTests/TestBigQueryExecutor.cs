using AutoDbPerf.Implementations;
using AutoDbPerf.Implementations.BigQuery;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class TestBigQueryExecutor
    {
        public TestBigQueryExecutor()
        {
            _queryExecutor = new BigQueryExecutor(new LoggerFactory(), new Context());
            _cachedQueryExecutor = new Query(_queryExecutor);
        }


        private readonly BigQueryExecutor? _queryExecutor;
        private readonly Query _cachedQueryExecutor;

        private class Context : IContext
        {
            public string GetEnv(ContextKey contextKey)
            {
                return contextKey switch
                {
                    ContextKey.GOOGLEPROJECTID => "gfk-eco-sandbox-red",
                    ContextKey.GOOGLECREDPATH => "Resources/gcreds/gcreds.json",
                    _ => ""
                };
            }
        }

        private class Query
        {
            private readonly IQueryExecutor _bigQueryExecutor;
            private QueryResult _query;

            public Query(IQueryExecutor bigQueryExecutor)
            {
                _bigQueryExecutor = bigQueryExecutor;
            }

            public QueryResult ExecuteQuery(string queryPath, string scenario, int timeout)
            {
                if (_query != null) return _query;

                var query = _bigQueryExecutor.ExecuteQuery(queryPath, scenario, timeout);
                _query = query;
                return query;
            }
        }

        [Test]
        public void WillReturnQueryResult_WithNoErrors()
        {
            var sut = _cachedQueryExecutor?.ExecuteQuery("Resources/BigQueries/usa-names.sql", "scenario", 60000);
            Assert.That(sut?.Problem, Is.Empty);
        }

        [Test]
        public void WillReturnQueryResult_WithCorrectQueryName()
        {
            var sut = _cachedQueryExecutor?.ExecuteQuery("Resources/BigQueries/usa-names.sql", "scenario", 60000);
            Assert.That(sut?.Query, Is.EqualTo("usa-names"));
        }

        [Test]
        public void WillReturnQueryResult_WithCorrectScenario()
        {
            var sut = _cachedQueryExecutor?.ExecuteQuery("Resources/BigQueries/usa-names.sql", "scenario", 60000);
            Assert.That(sut?.Scenario, Is.EqualTo("scenario"));
        }

        [Test]
        public void WillReturnQueryResult_WithProblem_WhenError()
        {
            var sut = _queryExecutor?.ExecuteQuery("Resources/BigQueries/error.sql", "scenario", 60000);
            Assert.That(sut?.Problem.Length, Is.GreaterThan(0));
        }

        [Test]
        public void WillReturnQueryResult_WithoutProblem_WhenNoError()
        {
            var sut = _cachedQueryExecutor?.ExecuteQuery("Resources/BigQueries/usa-names.sql", "scenario", 60000);
            Assert.That(sut?.Problem.Length, Is.EqualTo(0));
        }

        [Test]
        public void QueryWillTimeout()
        {
            var sut = _queryExecutor?.ExecuteQuery("Resources/BigQueries/usa-names.sql", "scenario", 0);
            Assert.That(sut?.Problem, Is.EqualTo("Timeout at 0ms"));
        }
    }
}