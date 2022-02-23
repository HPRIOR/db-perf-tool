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
                services.AddTransient<ITableOutput, CsvOutput>();
                services.AddTransient<IQueryExecutor, CliQueryExecutor>();
                services.AddTransient<IDirectoryScanner, DirectoryScanner>();
                services.AddTransient<ICommandExecutor, CommandExecutor>();
                services.AddTransient<ICommandGenerator, PgCommandGenerator>();
                services.AddTransient<ICommandGenerator, PgCommandGenerator>();
                services.AddTransient<IQueryInterpreter, PgQueryInterpreter>();
                services.AddTransient<IColumnOrderer, ColumnOrderer>();
                services.AddTransient<IQueryResultAggregator, PgQueryResultAggregator>();
                services.AddTransient<IQueryResultsAnalyser, QueryResultsAnalyser>();
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
        public void ProducesTheCorrectOutput()
        {
            var sqlPath = "Resources/TestBenchmark";
            var avgPrecision = 5;
            var timeout = 5000;
            var result = _benchmarker.GetBenchmarks(sqlPath, avgPrecision, timeout);
            var expected = "scenarios,scenario1,,,,,,,,scenario2\n" +
                           ",AvgExecutionTime,StdDev,Min,Max,AvgPlanningTime,StdDev,Min,Max,AvgExecutionTime,StdDev,Min,Max,AvgPlanningTime,StdDev,Min,Max\n" +
                           "query1,0.1,0,0.1,0.1,0.1,0,0.1,0.1,0.4,0,0.4,0.4,0.4,0,0.4,0.4\n" +
                           "query2,0.2,0,0.2,0.2,0.2,0,0.2,0.2,0.5,0,0.5,0.5,0.5,0,0.5,0.5\n" +
                           "query3,0.3,0,0.3,0.3,0.3,0,0.3,0.3,0.6,0,0.6,0.6,0.6,0,0.6,0.6\n";
            Assert.That(result, Is.EqualTo(expected));
        }


        [Test]
        public void CompletesWithNoExplainAnalyse()
        {
            var sqlPath = "Resources/NoExplainAnalyse";
            var errorMessage = "Error - see logs";
            var avgPrecision = 5;
            var timeout = 5000;
            var result = _benchmarker.GetBenchmarks(sqlPath, avgPrecision, timeout);
            var expected =
                "scenarios,scenario1,scenario2\n" +
                ",Error,Error\n" +
                $"query1,{errorMessage},N/A\n" +
                $"query2,{errorMessage},N/A\n" +
                $"query3,N/A,{errorMessage}\n" +
                $"query4,N/A,{errorMessage}\n";
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}