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
            _commandExecutor = Substitute.For<IQueryExecutor>();
            _commandExecutor.ExecuteQuery("query1", "scenario1", 5000)
                .Returns(new QueryResult("query1", "scenario1", null, null));
            _commandExecutor.ExecuteQuery("query2", "scenario2", 5000)
                .Returns(new QueryResult( "query2", "scenario2", null, null));
            _commandExecutor.ExecuteQuery("query3", "scenario3", 5000)
                .Returns(new QueryResult( "query3", "scenario3", null, null));

            _queryResultInterpreter = Substitute.For<IQueryResultInterpreter>();
            
        }

        private IQueryExecutor? _commandExecutor;
        private IDirectoryScanner? _directoryScanner;
        private IQueryResultInterpreter? _queryResultInterpreter;

        // [Test]
        // public void GivesCorrectNumberOfQueryResults_WithAvgPrecisionOfTwo()
        // {
        //     var queryExecutor =
        //         new QueryManager(new LoggerFactory(), new Context(new ConfigurationManager()), _commandExecutor, _directoryScanner, _queryResultInterpreter);
        //
        //     var result = queryExecutor.GetQueryResults("", 2);
        //     Assert.That(result.Count(), Is.EqualTo(6));
        // }
        //
        // [Test]
        // public void ErrorIsThrown_WithZeroAvgPrecision()
        // {
        //     var queryExecutor =
        //         new QueryManager(new LoggerFactory(), new Context(new ConfigurationManager()), _commandExecutor, _directoryScanner, _queryResultInterpreter);
        //     Assert.Throws<ArgumentException>(() =>
        //         queryExecutor.GetQueryResults("", 0)
        //     );
        // }
    }
}