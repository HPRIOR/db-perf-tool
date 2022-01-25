using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;
using Microsoft.Extensions.Logging;

namespace AutoDbPerf.Implementations.ClickHouse
{
    public class ClickhouseQueryExecutor : IQueryExecutor
    {
        private readonly IContext _ctx;
        private readonly ILogger<ClickhouseQueryExecutor> _logger;

        public ClickhouseQueryExecutor(ILoggerFactory loggerFactory, IContext ctx)
        {
            _ctx = ctx;
            _logger = loggerFactory.CreateLogger<ClickhouseQueryExecutor>();
        }

        private Task<ClickHouseCommandResult> ExecuteCommand(string queryPath)
        {
            return Task.Run(() =>
            {
                var stderr = GetStdError(queryPath);
                return StdErrorContainsError(stderr.Split())
                    ? new ClickHouseCommandResult(stderr, 0)
                    : new ClickHouseCommandResult("", float.Parse(stderr.Replace("\\n", "")));
            });
        }

        public QueryResult ExecuteQuery(string queryPath, string scenario, int timeout)
        {
            var queryName = queryPath.GetQueryNameFromPath();
            var cmdTask = ExecuteCommand(queryPath);
            _logger.LogInformation("Executing : {}-{}", scenario, queryName);
            if (cmdTask.Wait(timeout))
            {
                var cmdResult = cmdTask.Result;
                if (cmdResult.Problem.Any())
                {
                    _logger.LogError("Error occured during query Execution: {}", cmdResult.Problem);
                    return new QueryResult(0, 0, queryName, scenario,
                        cmdResult.Problem);
                }

                _logger.LogInformation("{}-{} - Execution time: {}", scenario, queryName, cmdResult.Time/1000);
                return new QueryResult(0, cmdResult.Time/1000, queryName, scenario);
            }

            _logger.LogWarning("Command timeout");
            return new QueryResult(0, 0, queryName, scenario, $"Timeout at {timeout}ms");
        }

        private string GetStdError(string queryPath)
        {
            var command = _ctx.GetEnv(ContextKey.ALTCHCLIENT) == "false" ? "clickhouse-client" : "clickhouse client";
            var host = _ctx.GetEnv(ContextKey.HOST).Any() ? _ctx.GetEnv(ContextKey.HOST) : "localhost";
            var interpolatedCommand = $"-c \"{command} --queries-file '{queryPath}' --time --host={host}\"";
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = interpolatedCommand,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = Process.Start(psi);

            var readTask = process?.StandardError.ReadToEndAsync();
            var ignoreStdOutTask = IgnoreStream(process?.StandardOutput);
            var processTask = process?.WaitForExitAsync();
            Task.WaitAll(readTask!, processTask!, ignoreStdOutTask);
            return readTask.Result;
        }

        // The Process API seems to hang on large process outputs unless you consume the stream asynchronously 
        private async Task IgnoreStream(StreamReader stream)
        {
            while (await stream?.ReadLineAsync() != null)
            {
            }
        }

        private bool StdErrorContainsError(IEnumerable<string> stderr) => stderr.Count(s => s.Any()) > 1;


        private record ClickHouseCommandResult(string Problem, float Time);
    }
}