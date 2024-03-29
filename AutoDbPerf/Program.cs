﻿using System;
using System.IO;
using System.Net.Http;
using AutoDbPerf.Implementations;
using AutoDbPerf.Implementations.BigQuery;
using AutoDbPerf.Implementations.ClickHouse;
using AutoDbPerf.Implementations.Elastic;
using AutoDbPerf.Implementations.Postgres;
using AutoDbPerf.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutoDbPerf
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureHostConfiguration(builder =>
                {
                    builder.AddJsonFile("appsettings.json", false);
                    builder.AddEnvironmentVariables();
                })
                .ConfigureServices((ctx, services) =>
                {
                    services.AddTransient<IQueryManager, QueryManager>();
                    services.AddTransient<ITableOutput, CsvOutput>();
                    services.AddTransient<IDirectoryScanner, DirectoryScanner>();
                    services.AddTransient<IQueryResultsAnalyser, QueryResultsAnalyser>();
                    services.AddTransient<IColumnOrderer, ColumnOrderer>();
                    services.AddSingleton<IContext, Context>();
                    services.AddLogging();
                    BuildWith(ctx.Configuration, services);
                }).Build();


            var benchmarker = ActivatorUtilities.CreateInstance<Benchmarker>(host.Services);
            var context = ActivatorUtilities.CreateInstance<Context>(host.Services);

            var result = benchmarker.GetBenchmarks(
                context.GetEnv(ContextKey.QUERYPATH),
                int.Parse(context.GetEnv(ContextKey.AVGPRECISION)),
                int.Parse(context.GetEnv(ContextKey.TIMEOUT)) * 1000 * 60
            );

            File.WriteAllText(Path.Join(context.GetEnv(ContextKey.OUTPUTDIR), "output.csv"), result);
        }

        private static void BuildWith(IConfiguration config, IServiceCollection services)
        {
            var target = config["TARGET"];
            switch (target)
            {
                case "postgres":
                    services.AddTransient<IQueryExecutor, CliQueryExecutor>();
                    services.AddTransient<ICommandExecutor, CommandExecutor>();
                    services.AddTransient<IQueryInterpreter, PgQueryInterpreter>();
                    services.AddTransient<ICommandGenerator, PgCommandGenerator>();
                    services.AddTransient<IQueryResultAggregator, PgQueryResultAggregator>();
                    return;
                case "elastic":
                    services.AddTransient<IQueryExecutor, CliQueryExecutor>();
                    services.AddTransient<ICommandExecutor, CommandExecutor>();
                    services.AddTransient<IQueryInterpreter, ElasticQueryInterpreter>();
                    services.AddTransient<ICommandGenerator, ElasticCommandGenerator>();
                    services.AddTransient<IQueryResultAggregator, BasicQueryResultAggregator>();
                    return;
                case "bq":
                    services.AddTransient<IQueryExecutor, BigQueryExecutor>();
                    services.AddTransient<IQueryResultAggregator, BqQueryResultAggregator>();
                    return;
                case "clickhouse":
                    services.AddTransient<IQueryExecutor, ClickhouseQueryExecutor>();
                    services.AddTransient<IQueryResultAggregator, BasicQueryResultAggregator>();
                    services.AddSingleton<HttpClient>();
                    return;
                default:
                    throw new ArgumentException($"Incorrect target set in configuration: {target}");
            }
        }
    }
}