using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Logging;

namespace AutoDbPerf.Implementations.BigQuery
{
    public class BigQueryExecutor : IQueryExecutor
    {
        private readonly IContext _ctx;
        private readonly ILogger<BigQueryExecutor> _logger;

        public BigQueryExecutor(ILoggerFactory loggerFactory, IContext ctx)
        {
            _logger = loggerFactory.CreateLogger<BigQueryExecutor>();
            _ctx = ctx;
        }

        public QueryResult ExecuteQuery(string queryPath, string scenario, int timeout)
        {
            var queryName = queryPath.Split("/").Last().Split(".").First();
            var queryTask = QueryTask(queryPath);
            _logger.LogInformation("Executing : {}-{}", scenario, queryName);
            if (queryTask.Wait(timeout))
            {
                var cmdResult = queryTask.Result;
                if (cmdResult.Problem.Length > 0)
                {
                    _logger.LogError("Error occured during query execution: {}. JobId: {}", cmdResult.Problem,
                        cmdResult.JobId);
                    return new QueryResult(0, 0, queryName, scenario, cmdResult.Problem);
                }

                _logger.LogInformation("{}-{} - Execution time: {}. JobId: {}", scenario, queryName, cmdResult.Time,
                    cmdResult.JobId);
                return new QueryResult(0, cmdResult.Time, queryName, scenario);
            }

            _logger.LogWarning("Command timout");
            return new QueryResult(0, 0, queryName, scenario, $"Timeout at {timeout}ms");
        }

        private Task<BqCommandResult> QueryTask(string queryPath)
        {
            return Task.Run(() =>
            {
                //TODO use polly
                try
                {
                    var projectId = _ctx.GetEnv(ContextKey.GOOGLEPROJECTID);
                    var credential = GoogleCredential.FromFile(_ctx.GetEnv(ContextKey.GOOGLECREDPATH));
                    var client = BigQueryClient.Create(projectId, credential);

                    var query = File.ReadAllText(queryPath);

                    var queryOptions = new QueryOptions { UseQueryCache = false };
                    var queryJob = client.CreateQueryJob(query, null, queryOptions);

                    var job = queryJob.PollUntilCompleted();
                    var jobId = queryJob.Reference.JobId;

                    var errors = queryJob.Status.Errors;
                    if (errors != null) return new BqCommandResult(errors.Last().Message, 0, jobId);

                    var queryTime = job.Statistics.EndTime - job.Statistics.StartTime;
                    return new BqCommandResult("", (float)queryTime, jobId);
                }
                catch (AggregateException ae)
                {
                    return new BqCommandResult(ae.ToString(), 0, "N/A");
                }
                catch (HttpRequestException httpe)
                {
                    return new BqCommandResult(httpe.ToString(), 0, "N/A");
                }
                catch (IOException ioe)
                {
                    return new BqCommandResult(ioe.ToString(), 0, "N/A");
                }
            });
        }

        private record BqCommandResult
        {
            public BqCommandResult(string problem, float time, string jobId)
            {
                Time = time;
                JobId = jobId;
                Problem = problem;
            }

            public string JobId { get; }
            public float Time { get; }
            public string Problem { get; }
        }
    }
}