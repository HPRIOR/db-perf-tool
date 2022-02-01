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
                services.AddTransient<IResultAnalyser, ResultAnalyser>();
                services.AddTransient<ITableOutput, CsvOutput>();
                services.AddSingleton<IContext>(new Context(order));
                services.AddTransient<IQueryExecutor, CliQueryExecutor>();
                services.AddTransient<ICommandExecutor, CommandExecutor>();
                services.AddTransient<ICommandGenerator, PostgresCommandGenerator>();
                services.AddTransient<IQueryInterpreter, PostgresQueryInterpreter>();
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
            var result = _queryManager
                .GetQueryResult("Resources/PostgresSQL", 1, 1000)
                .OrderBy(x => x.Query)
                .ThenBy(x => x.Scenario)
                .ToArray();

            Assert.That(result[0].Query, Is.EqualTo("query1"));
            Assert.That(result[0].Scenario, Is.EqualTo("scenario1"));
            Assert.That(result[0].Problem, Is.EqualTo(""));

            Assert.That(result[1].Query, Is.EqualTo("query2"));
            Assert.That(result[1].Scenario, Is.EqualTo("scenario1"));
            Assert.That(result[1].Problem, Is.EqualTo(""));

            Assert.That(result[2].Query, Is.EqualTo("query3"));
            Assert.That(result[2].Scenario, Is.EqualTo("scenario2"));
            Assert.That(result[2].Problem, Is.EqualTo(""));

            Assert.That(result[3].Query, Is.EqualTo("query4"));
            Assert.That(result[3].Scenario, Is.EqualTo("scenario2"));
            Assert.That(result[3].Problem, Is.EqualTo(""));
        }

        [Test]
        public void HasCorrectNumberOfResults()
        {
            var result = _queryManager
                .GetQueryResult("Resources/PostgresSQL", 1, 1000)
                .OrderBy(x => x.Query)
                .ThenBy(x => x.Scenario)
                .ToArray();

            Assert.That(result.Length, Is.EqualTo(4));
        }
        
        [Test]
        public void WithAvgPrecisionOfTwo_HasCorrectNumberOfResult()
        {
            var result = _queryManager
                .GetQueryResult("Resources/PostgresSQL", 2, 1000)
                .OrderBy(x => x.Query)
                .ThenBy(x => x.Scenario)
                .ToArray();

            Assert.That(result.Length, Is.EqualTo(8));
        }
        

        [Test]
        public void ReturnsCorrectRoundRobinOrder()
        {
            var queryManager = GetQueryManger("rr");
            var result = queryManager
                .GetQueryResult("Resources/PostgresSQL", 2, 1000).ToArray();

            Assert.That(result[0].Query, Is.EqualTo("query1"));
            Assert.That(result[0].Scenario, Is.EqualTo("scenario1"));

            Assert.That(result[1].Query, Is.EqualTo("query2"));
            Assert.That(result[1].Scenario, Is.EqualTo("scenario1"));
            
            Assert.That(result[2].Query, Is.EqualTo("query1"));
            Assert.That(result[2].Scenario, Is.EqualTo("scenario1"));

            Assert.That(result[3].Query, Is.EqualTo("query2"));
            Assert.That(result[3].Scenario, Is.EqualTo("scenario1"));
            
            Assert.That(result[4].Query, Is.EqualTo("query3"));
            Assert.That(result[4].Scenario, Is.EqualTo("scenario2"));

            Assert.That(result[5].Query, Is.EqualTo("query4"));
            Assert.That(result[5].Scenario, Is.EqualTo("scenario2"));
            
            Assert.That(result[6].Query, Is.EqualTo("query3"));
            Assert.That(result[6].Scenario, Is.EqualTo("scenario2"));

            Assert.That(result[7].Query, Is.EqualTo("query4"));
            Assert.That(result[7].Scenario, Is.EqualTo("scenario2"));
        }
        
        
        [Test]
        public void ReturnsCorrectSequentialOrder()
        {
            var queryManager = GetQueryManger("seq");
            var result = queryManager
                .GetQueryResult("Resources/PostgresSQL", 2, 1000).ToArray();

            Assert.That(result[0].Query, Is.EqualTo("query1"));
            Assert.That(result[0].Scenario, Is.EqualTo("scenario1"));

            Assert.That(result[1].Query, Is.EqualTo("query1"));
            Assert.That(result[1].Scenario, Is.EqualTo("scenario1"));
            
            Assert.That(result[2].Query, Is.EqualTo("query2"));
            Assert.That(result[2].Scenario, Is.EqualTo("scenario1"));

            Assert.That(result[3].Query, Is.EqualTo("query2"));
            Assert.That(result[3].Scenario, Is.EqualTo("scenario1"));
            
            Assert.That(result[4].Query, Is.EqualTo("query3"));
            Assert.That(result[4].Scenario, Is.EqualTo("scenario2"));

            Assert.That(result[5].Query, Is.EqualTo("query3"));
            Assert.That(result[5].Scenario, Is.EqualTo("scenario2"));
            
            Assert.That(result[6].Query, Is.EqualTo("query4"));
            Assert.That(result[6].Scenario, Is.EqualTo("scenario2"));

            Assert.That(result[7].Query, Is.EqualTo("query4"));
            Assert.That(result[7].Scenario, Is.EqualTo("scenario2"));
        }
    }
}