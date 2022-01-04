using System;
using System.Linq;
using AutoDbPerf.Implementations;
using AutoDbPerf.Implementations.Postgres;
using AutoDbPerf.Interfaces;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class TestPostgresCommandExecutor
    {
        [SetUp]
        public void Setup()
        {
            var ctx = new Context();
            _commandExecutor = new CommandExecutor(new PostgresCommandGenerator(ctx));
        }

        private class Context : IContext
        {
            public string GetEnv(ContextKey contextKey)
            {
                return contextKey switch
                {
                    ContextKey.PGPASSWORD => "docker",
                    ContextKey.PGUSER => "postgres",
                    ContextKey.PGNAME => "dvdrental",
                    ContextKey.PGHOST => "localhost",
                    ContextKey.PGPORT => "5432",
                    _ => ""
                };
            }
        }

        private ICommandExecutor? _commandExecutor;
        private const string Query = "Resources/PostgresSQL/scenario1/query1.sql";
        private const string Error = "error";

        private readonly Predicate<string> _initialCommandPredicate =
            str => str.Contains("Planning") || str.Contains("Execution");


        [Test]
        public void WillReturnStdError()
        {
            var sut = _commandExecutor?.ExecuteCommand(Error, _initialCommandPredicate);
            Assert.That(sut.Result.Stderr.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void WillNotReturnStdError_IfNotError()
        {
            var sut = _commandExecutor?.ExecuteCommand(Query, _initialCommandPredicate);
            Assert.That(sut.Result.Stderr.Count(), Is.EqualTo(0));
        }

        [Test]
        public void WillReturnStdout_WithPlanningString()
        {
            var sut = _commandExecutor?.ExecuteCommand(Query, _initialCommandPredicate);
            Assert.That(sut.Result.Stdout.Any(s => s.Contains("Planning")), Is.True);
        }

        [Test]
        public void WillReturnStdout_WithExecutionString()
        {
            var sut = _commandExecutor?.ExecuteCommand(Query, _initialCommandPredicate);
            Assert.That(sut.Result.Stdout.Any(s => s.Contains("Execution")), Is.True);
        }
    }
}