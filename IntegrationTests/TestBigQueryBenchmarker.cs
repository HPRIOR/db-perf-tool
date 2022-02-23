using System;
using System.Linq;
using AutoDbPerf.Implementations;
using AutoDbPerf.Implementations.BigQuery;
using AutoDbPerf.Implementations.Postgres;
using AutoDbPerf.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class TestBigQueryBenchmarker
    {
        [SetUp]
        public void Setup()
        {
            var host = Host.CreateDefaultBuilder().ConfigureServices((_, services) =>
            {
                services.AddTransient<IQueryManager, QueryManager>();
                services.AddTransient<ITableOutput, CsvOutput>();
                services.AddTransient<IQueryExecutor, BigQueryExecutor>();
                services.AddTransient<IDirectoryScanner, DirectoryScanner>();
                services.AddTransient<IColumnOrderer, ColumnOrderer>();
                services.AddTransient<IQueryResultsAnalyser, QueryResultsAnalyser>();
                services.AddTransient<IQueryResultAggregator, BqQueryResultAggregator>();
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
                    ContextKey.GOOGLECREDPATH => "Resources/gcreds/gcreds.json",
                    ContextKey.GOOGLEPROJECTID => "gfk-eco-sandbox-red",
                    _ => ""
                };
            }
        }

        [Test]
        public void WillNotContainErrorsInResult()
        {
            var sqlPath = "Resources/bigquery-benchmark";
            var avgPrecision = 1;
            var timeout = 10000;
            var result = _benchmarker.GetBenchmarks(sqlPath, avgPrecision, timeout);
            Assert.That(result.Contains("error"), Is.False);
        }

        [Test]
        public void WillContainErrorsInResult()
        {
             var sqlPath = "Resources/bigquery-error";
             var avgPrecision = 1;
             var timeout = 10000;
             var result = _benchmarker.GetBenchmarks(sqlPath, avgPrecision, timeout);
             Console.WriteLine(result);
            Assert.That(result.Contains("Error"), Is.True);
        }
        
        
        
    }
}