using System;
using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Implementations;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace test_auto_db_perf
{
    [TestFixture]
    public class TestQueryManager
    {
        [SetUp]
        public void Setup()
        {
            _commandExecutor = Substitute.For<IQueryExecutor>();
            _commandExecutor.ExecuteQuery("query1", "scenario1", 5000)
                .Returns(new QueryResult("query1", "scenario1", null, null));
            _commandExecutor.ExecuteQuery("query2", "scenario2", 5000)
                .Returns(new QueryResult("query2", "scenario2", null, null));
            _commandExecutor.ExecuteQuery("query3", "scenario3", 5000)
                .Returns(new QueryResult("query3", "scenario3", null, null));

        }

        private IQueryExecutor? _commandExecutor;

        [Test]
        public void GivesCorrectNumberOfQueryResults_WithAvgPrecisionOfTwo()
        {
            var queryExecutor =
                new QueryManager(new AutoDbPerf.Implementations.Context(new ConfigurationManager()), _commandExecutor);

            var queryInfo = new List<QueryInfo>
            {
                new("scenario1", new[] { "query1" }),
                new("scenario2", new[] { "query2" }),
                new("scenario3", new[] { "query3" })
            };
            var result = queryExecutor.GetQueryResults(queryInfo, 2);
            Assert.That(result.Count(), Is.EqualTo(6));
        }

        [Test]
        public void ErrorIsThrown_WithZeroAvgPrecision()
        {
            var queryExecutor =
                new QueryManager(new AutoDbPerf.Implementations.Context(new ConfigurationManager()), _commandExecutor);

            var queryInfo = new List<QueryInfo>
            {
                new("scenario1", new[] { "query1" }),
                new("scenario2", new[] { "query2" }),
                new("scenario3", new[] { "query3" })
            };
            Assert.Throws<ArgumentException>(() =>
                queryExecutor.GetQueryResults(queryInfo, 0)
            );
        }


        private IContext GetNewContext(string order)
        {
            return new Context(order);
        }

        private class Context : IContext
        {
            private readonly string _order;

            internal Context(string order)
            {
                _order = order;
            }

            public string GetEnv(ContextKey contextKey) => contextKey switch
            {
                ContextKey.ORDER => _order
            };

        }

        [Test]
        public void QueryResults_AreOrderedBy_RoundRobin()
        {
            var queryExecutor =
                new QueryManager(GetNewContext("rr"), _commandExecutor);

            var queryInfo = new List<QueryInfo>
            {
                new("scenario1", new[] { "query1" }),
                new("scenario2", new[] { "query2" }),
                new("scenario3", new[] { "query3" })
            };
            
            Assert.Throws<ArgumentException>(() =>
                queryExecutor.GetQueryResults(queryInfo, 1)
            );

        }

        [Test]
        public void QueryResults_AreOrderedBy_Sequence()
        {
            var queryExecutor =
                new QueryManager(GetNewContext("seq"), _commandExecutor);

            var queryInfo = new List<QueryInfo>
            {
                new("scenario1", new[] { "query1" }),
                new("scenario2", new[] { "query2" }),
                new("scenario3", new[] { "query3" })
            };
            Assert.Throws<ArgumentException>(() =>
                queryExecutor.GetQueryResults(queryInfo, 1)
            );
        }
        
    }
}