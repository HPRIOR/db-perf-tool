using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;
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
            var queryName = queryPath.GetQueryNameFromPath();
            var queryTask = QueryTask(queryPath);
            _logger.LogInformation("Executing : {}-{}", scenario, queryName);
            if (queryTask.Wait(timeout))
            {
                var cmdResult = queryTask.Result;
                if (cmdResult.Problem.Length > 0)
                {
                    _logger.LogError("Error occured during query execution: {}. JobId: {}", cmdResult.Problem,
                        cmdResult.JobId);
                    return new QueryResult(scenario, queryName, null, null, true, cmdResult.Problem);
                }

                _logger.LogInformation(
                    "{}-{} - Execution time: {}ms. Processed: {}gb. JobId: {}, BIEngineMode: {}, BytesBilled: {}",
                    scenario,
                    queryName, cmdResult.Time, cmdResult.ByteProcessed,
                    cmdResult.JobId, cmdResult.BiEngineMode, cmdResult.BytesBilled);
                var numData = new Dictionary<Data, float>
                {
                    { Data.EXECUTION_TIME, cmdResult.Time },
                    { Data.BYTES_PROCESSED, cmdResult.ByteProcessed },
                    { Data.BYTES_BILLED, cmdResult.BytesBilled }
                };
                var strData = new Dictionary<Data, string> { { Data.BI_MODE, cmdResult.BiEngineMode } };
                return new QueryResult(scenario, queryName, numData, strData);
            }

            _logger.LogWarning("Command timout");
            return new QueryResult(scenario, queryName, null, null, true, $"Timeout at {timeout}ms");
        }

        private Task<BqCommandResult> QueryTask(string queryPath)
        {
            return Task.Run(() =>
            {
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
                    if (errors != null) return new BqCommandResult(errors.Last().Message, 0, jobId, 0, "", 0);

                    var bytesProcessed = job.Statistics?.Query?.TotalBytesProcessed / 1073741824d ?? 0;
                    var biEngineMode = job.Statistics?.Query?.BiEngineStatistics?.BiEngineMode ?? "NONE";
                    var bytesBilled = job.Statistics?.Query?.TotalBytesBilled ?? 0;
                    var queryTime = (job.Statistics?.EndTime - job.Statistics?.StartTime) ?? 0;
                    return new BqCommandResult("", queryTime, jobId, (float)bytesProcessed, biEngineMode,
                        bytesBilled);
                }
                catch (AggregateException ae)
                {
                    return new BqCommandResult(ae.ToString(), 0, "N/A", 0, "", 0);
                }
                catch (HttpRequestException httpe)
                {
                    return new BqCommandResult(httpe.ToString(), 0, "N/A", 0, "", 0);
                }
                catch (IOException ioe)
                {
                    return new BqCommandResult(ioe.ToString(), 0, "N/A", 0, "", 0);
                }
            });
        }

        private record BqCommandResult(string Problem, float Time, string JobId, float ByteProcessed,
            string BiEngineMode, long BytesBilled);
    }
}