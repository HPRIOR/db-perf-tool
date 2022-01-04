using System;
using System.Linq;
using AutoDbPerf.Implementations;
using AutoDbPerf.Implementations.Elastic;
using AutoDbPerf.Interfaces;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class TestElasticCommandExecutor
    {
        [SetUp]
        public void Setup()
        {
            var ctx = new Context();
            _commandExecutor = new CommandExecutor(new ElasticCommandGenerator(ctx));
            _queryInterpreter = new ElasticQueryInterpreter();
        }

        private class Context : IContext
        {
            public string GetEnv(ContextKey contextKey)
            {
                return contextKey switch
                {
                    ContextKey.ELASTICINDEX => "test-index",
                    _ => ""
                };
            }
        }


        private ICommandExecutor? _commandExecutor;
        private ElasticQueryInterpreter _queryInterpreter;
        private const string Query = "Resources/elastic/scenario1/query1.json";
        private const string Error = "Resources/elastic/scenario1/error";
        private const string ParseError = "Resources/elastic/scenario1/parseError.json";
        private const string QueryError = "Resources/elastic/scenario1/queryError.json";


        [Test]
        public void WillNotReturnStdError_IfNotError()
        {
            var sut = _commandExecutor?.ExecuteCommand(Query, _queryInterpreter.InitialScanPredicate());
            Console.WriteLine("stdout");
            sut.Result.Stdout.ToList().ForEach(Console.WriteLine);
            Console.WriteLine("stderr");
            sut.Result.Stderr.ToList().ForEach(Console.WriteLine);
            Assert.That(sut.Result.Stderr.Count(), Is.EqualTo(0));
        }

        [Test]
        public void WillCatchJsonParseException()
        {
            var sut = _commandExecutor?.ExecuteCommand(ParseError, _queryInterpreter.InitialScanPredicate());
            Console.WriteLine("stdout");
            sut.Result.Stdout.ToList().ForEach(Console.WriteLine);
            Console.WriteLine("stderr");
            sut.Result.Stderr.ToList().ForEach(Console.WriteLine);
            Assert.That(sut.Result.Stdout.Any(x => x.Contains("json_parse_exception")), Is.True);
        }

        [Test]
        public void WillReturnStdout_WithTookString()
        {
            var sut = _commandExecutor?.ExecuteCommand(Query, _queryInterpreter.InitialScanPredicate());
            Console.WriteLine("stdout");
            sut.Result.Stdout.ToList().ForEach(Console.WriteLine);
            Console.WriteLine("stderr");
            sut.Result.Stderr.ToList().ForEach(Console.WriteLine);
            Assert.That(sut.Result.Stdout.Any(s => s.Contains("took")), Is.True);
        }

        [Test]
        public void WillCatch_ElasticQueryError()
        {
            var sut = _commandExecutor?.ExecuteCommand(QueryError, _queryInterpreter.InitialScanPredicate());
            Console.WriteLine("stdout");
            sut.Result.Stdout.ToList().ForEach(Console.WriteLine);
            Console.WriteLine("stderr");
            sut.Result.Stderr.ToList().ForEach(Console.WriteLine);
            Assert.That(sut.Result.Stdout.Any(x => x.Contains("parsing_exception")), Is.True);
        }
    }
}