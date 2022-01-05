using System.Collections.Generic;
using System.Diagnostics;
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

        public QueryResult ExecuteQuery(string queryPath, string scenario, int timeout)
        {
            var queryName = queryPath.GetQueryNameFromPath();
            var cmdResult = ExecuteCommand(queryPath);
            if (cmdResult.Problem.Any())
            {
                _logger.LogError("Error occured during query Execution: {}", cmdResult.Problem);
                return new QueryResult(0, 0, queryName, scenario,
                    cmdResult.Problem);
            }

            _logger.LogInformation("{}-{} - Execution time: {}", scenario, queryName, cmdResult.Time);
            return new QueryResult(0, cmdResult.Time, queryName, scenario);
        }

        private string GetStdError(string queryPath)
        {
            var localCommandDiff = _ctx.GetEnv(ContextKey.DOCKER) == "true" ? "-" : " ";
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"clickhouse{localCommandDiff}client --time --queries-file '{queryPath}'\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = Process.Start(psi);

            var readTask = process?.StandardError.ReadToEndAsync();
            var processTask = process?.WaitForExitAsync();
            Task.WaitAll(readTask!, processTask!);
            return readTask.Result;
        }

        private bool StdErrorContainsError(IEnumerable<string> stderr) => stderr.Count(s => s.Any()) > 1;

        private ClickHouseCommandResult ExecuteCommand(string queryPath)
        {
            var stderr = GetStdError(queryPath);
            return StdErrorContainsError(stderr.Split())
                ? new ClickHouseCommandResult(stderr, 0)
                : new ClickHouseCommandResult("", float.Parse(stderr.Replace("\\n", "")));
        }

        private record ClickHouseCommandResult(string Problem, float Time);
    }
}