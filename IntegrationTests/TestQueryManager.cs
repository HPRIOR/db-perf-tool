using System;
using System.Linq;
using AutoDbPerf.Implementations;
using AutoDbPerf.Implementations.Postgres;
using AutoDbPerf.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class TestQueryManager
    {
        [SetUp]
        public void Setup()
        {
            _queryManager = GetQueryManger("");
        }

        private IQueryManager GetQueryManger(string order)
        {
            var host = Host.CreateDefaultBuilder().ConfigureServices((_, services) =>
            {
                services.AddTransient<IQueryManager, QueryManager>();
                services.AddTransient<ITableOutput, CsvOutput>();
                services.AddSingleton<IContext>(new Context(order));
                services.AddTransient<IQueryExecutor, CliQueryExecutor>();
                services.AddTransient<ICommandExecutor, CommandExecutor>();
                services.AddTransient<ICommandGenerator, PgCommandGenerator>();
                services.AddTransient<IQueryInterpreter, PgQueryInterpreter>();
                services.AddTransient<IDirectoryScanner, DirectoryScanner>();
            }).Build();
            return ActivatorUtilities.CreateInstance<QueryManager>(host.Services);
        }

        private IQueryManager _queryManager;

        private class Context : IContext
        {
            private readonly string _order;

            public Context(string order)
            {
                _order = order;
            }

            public string GetEnv(ContextKey contextKey)
            {
                return contextKey switch
                {
                    ContextKey.PGPASSWORD => "docker",
                    ContextKey.PGUSER => "postgres",
                    ContextKey.PGNAME => "dvdrental",
                    ContextKey.PGHOST => "localhost",
                    ContextKey.PGPORT => "5432",
                    ContextKey.ORDER => _order,
                    _ => ""
                };
            }
        }

        [Test]
        public void ReturnsCorrectQueryResults()
        {
        }

        [Test]
        public void HasCorrectNumberOfResults()
        {
        }

        [Test]
        public void WithAvgPrecisionOfTwo_HasCorrectNumberOfResult()
        {
        }


        [Test]
        public void ReturnsCorrectRoundRobinOrder()
        {
        }


        [Test]
        public void ReturnsCorrectSequentialOrder()
        {
        }
    }
}