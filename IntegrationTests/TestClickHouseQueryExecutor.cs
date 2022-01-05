using System;
using AutoDbPerf.Implementations;
using AutoDbPerf.Implementations.BigQuery;
using AutoDbPerf.Implementations.ClickHouse;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class TestClickHouseQueryExecutor
    {
        public TestClickHouseQueryExecutor()
        {
            _queryExecutor = new ClickhouseQueryExecutor(new LoggerFactory(), new Context());
        }

        private readonly IQueryExecutor _queryExecutor;

        private class Context : IContext
        {
            public string GetEnv(ContextKey contextKey)
            {
                return contextKey switch
                {
                    ContextKey.DOCKER => "false",
                    _ => ""
                };
            }
        }

        [Test]
        public void WillReturnQueryResult_WithNoErrors()
        {
            var sut = _queryExecutor.ExecuteQuery("Resources/clickhouse/scenario1/query1.sql", "test", 500);
            Assert.That(sut.Problem, Is.Empty);
        }
        
        [Test]
        public void WillReturnQueryResult_WithTiming()
        {
            var sut = _queryExecutor.ExecuteQuery("Resources/clickhouse/scenario1/query1.sql", "test", 500);
            Assert.That(sut.ExecutionTime, Is.GreaterThan(0));
        }
        [Test]
        public void WillReturnError_WithBadCommand()
        {
            var sut = _queryExecutor.ExecuteQuery("Resources/clickhouse/scenario1/error.sql", "test", 500);
            Assert.That(sut.Problem, Is.Not.Empty);
        }
    }
}