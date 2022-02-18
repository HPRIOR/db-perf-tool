using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
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
        private readonly HttpClient _client;

        private string Url =>
            $"http://{_ctx.GetEnv(ContextKey.HOST)}:8123/?add_http_cors_header=1&user=default&password=&default_format=JSONCompact&max_result_rows=1000&max_result_bytes=10000000&result_overflow_mode=break";

        public ClickhouseQueryExecutor(ILoggerFactory loggerFactory, IContext ctx, HttpClient client)
        {
            _ctx = ctx;
            _logger = loggerFactory.CreateLogger<ClickhouseQueryExecutor>();
            _client = client;
        }

        private Task<ClickHouseCommandResult> ExecuteCommand(string queryPath)
        {
            return Task.Run(() =>
            {
                try
                {
                    var query = File.ReadAllText(queryPath);
                    var response = _client
                        .PostAsync(Url, new StringContent(query))
                        .Result
                        .Content
                        .ReadAsStringAsync()
                        .Result;
                    var jsonResponse = JsonSerializer.Deserialize<ClickhouseResponse>(response);
                    return new ClickHouseCommandResult("", (float)jsonResponse.Statistics.ElapsedMs);
                }
                catch (Exception ex)
                {
                    return new ClickHouseCommandResult(ex.ToString(), 0);
                }
            });
        }

        public QueryResult2 ExecuteQuery(string queryPath, string scenario, int timeout)
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
                    return new QueryResult2(scenario, queryName, null, true, cmdResult.Problem);
                }

                var cmdResultTime = cmdResult.Time;
                _logger.LogInformation("{}-{} - Execution time: {}", scenario, queryName, cmdResultTime);
                var data = new Dictionary<string, QueryData>();
                data.Add("ExecutionTime", new QueryData(cmdResultTime));
                return new QueryResult2(scenario, queryName, data);
            }

            _logger.LogWarning("Command timeout");
            return new QueryResult2(scenario, queryName, null, true, $"Timeout at {timeout}ms");
        }

        private record ClickHouseCommandResult(string Problem, float Time);
    }
}