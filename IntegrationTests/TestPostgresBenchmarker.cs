using AutoDbPerf.Implementations;
using AutoDbPerf.Implementations.Postgres;
using AutoDbPerf.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class TestPostgresBenchmarker
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
                services.AddTransient<ICommandGenerator, PostgresCommandGenerator>();
                services.AddTransient<ICommandGenerator, PostgresCommandGenerator>();
                services.AddTransient<IQueryInterpreter, PostgresQueryInterpreter>();
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
                    ContextKey.PGPASSWORD => "docker",
                    ContextKey.PGUSER => "postgres",
                    ContextKey.PGNAME => "dvdrental",
                    ContextKey.PGHOST => "localhost",
                    ContextKey.PGPORT => "5432",
                    _ => ""
                };
            }
        }


        [Test]
        public void ProducesTheCorrectOutPutWithNonApplicableCells()
        {
            var sqlPath = "Resources/TestBenchmark";
            var avgPrecision = 5;
            var timeout = 5000;
            var result = _benchmarker.GetBenchmarks(sqlPath, avgPrecision, timeout);
            var expected = "scenarios,scenario1,scenario2\n" +
                           "query1,Planning: 0.1 Execution: 0.1 Total: 0.2,Planning: 0.4 Execution: 0.4 Total: 0.8\n" +
                           "query2,Planning: 0.2 Execution: 0.2 Total: 0.4,Planning: 0.5 Execution: 0.5 Total: 1\n" +
                           "query3,Planning: 0.3 Execution: 0.3 Total: 0.6,Planning: 0.6 Execution: 0.6 Total: 1.2\n";
            Assert.That(result, Is.EqualTo(expected));
        }


        [Test]
        public void CompletesWithNoExplainAnalyse()
        {
            var sqlPath = "Resources/NoExplainAnalyse";
            var avgPrecision = 5;
            var timeout = 5000;
            var result = _benchmarker.GetBenchmarks(sqlPath, avgPrecision, timeout);
            var expected =
                "scenarios,scenario1,scenario2\n" +
                "query1,N/A,N/A\n" +
                "query2,N/A,N/A\n" +
                "query3,N/A,N/A\n" +
                "query4,N/A,N/A\n";
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}