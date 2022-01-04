using System;
using AutoDbPerf.Implementations;
using AutoDbPerf.Implementations.Elastic;
using AutoDbPerf.Implementations.Postgres;
using AutoDbPerf.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class TestElasticBenchmarker
    {
        [SetUp]
        public void Setup()
        {
            var host = Host.CreateDefaultBuilder().ConfigureServices((_, services) =>
            {
                services.AddTransient<IQueryManager, QueryManager>();
                services.AddTransient<IResultAnalyser, ResultAnalyser>();
                services.AddTransient<ITableOutput, CsvOutput>();
                services.AddTransient<IQueryExecutor, QueryExecutor>();
                services.AddTransient<IDirectoryScanner, DirectoryScanner>();
                services.AddTransient<ICommandExecutor, CommandExecutor>();
                services.AddTransient<ICommandGenerator, ElasticCommandGenerator>();
                services.AddTransient<IQueryInterpreter, ElasticQueryInterpreter>();
                services.AddSingleton<IContext>(new Context());
            }).Build();
            _benchmarker = ActivatorUtilities.CreateInstance<Benchmarker>(host.Services);
        }

        private static Benchmarker _benchmarker;

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


        [Test]
        public void DoesntFallOver()
        {
            var queryPath = "Resources/elastic-benchmark";
            var avgPrecision = 5;
            var timeout = 5000;
            _benchmarker.GetBenchmarks(queryPath, avgPrecision, timeout);
        }

        [Test]
        public void ProducesErrorMessagesInOutput()
        {
             var queryPath = "Resources/elastic-error";
             var avgPrecision = 5;
             var timeout = 5000;
             var result = _benchmarker.GetBenchmarks(queryPath, avgPrecision, timeout);
             var expected = "scenarios,scenario1,scenario2\nquery1,Error - see logs,Error - see logs\nquery2,Error - see logs,Error - see logs\n";
             Assert.That(result, Is.EqualTo(expected));
        }
    }
}