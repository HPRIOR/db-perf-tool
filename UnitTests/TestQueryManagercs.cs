using System;
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
            _directoryScanner = Substitute.For<IDirectoryScanner>();
            _directoryScanner.ScanDirectories("").Returns(new[]
            {
                new ScenarioQueryPath("scenario1", new[] { "query1" }),
                new ScenarioQueryPath("scenario2", new[] { "query2" }),
                new ScenarioQueryPath("scenario3", new[] { "query3" }),
            });

            _commandExecutor = Substitute.For<IQueryExecutor>();
            _commandExecutor.ExecuteQuery("query1", "scenario1", 5000)
                .Returns(new QueryResult(0, 0, "query1", "scenario1"));
            _commandExecutor.ExecuteQuery("query2", "scenario2", 5000)
                .Returns(new QueryResult(0, 0, "query2", "scenario2"));
            _commandExecutor.ExecuteQuery("query3", "scenario3", 5000)
                .Returns(new QueryResult(0, 0, "query3", "scenario3"));
        }

        private IQueryExecutor? _commandExecutor;
        private IDirectoryScanner? _directoryScanner;

        [Test]
        public void GivesCorrectNumberOfQueryResults_WithAvgPrecisionOfTwo()
        {
            var queryExecutor =
                new QueryManager(new LoggerFactory(), new Context(new ConfigurationManager()), _commandExecutor, _directoryScanner);

            var result = queryExecutor.GetQueryResult("", 2);
            Assert.That(result.Count(), Is.EqualTo(6));
        }

        [Test]
        public void ErrorIsThrown_WithZeroAvgPrecision()
        {
            var queryExecutor =
                new QueryManager(new LoggerFactory(), new Context(new ConfigurationManager()), _commandExecutor, _directoryScanner);
            Assert.Throws<ArgumentException>(() =>
                queryExecutor.GetQueryResult("", 0)
            );
        }
    }
}