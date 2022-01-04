using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;
using Microsoft.Extensions.Logging;

namespace AutoDbPerf.Implementations
{
    public class QueryExecutor : IQueryExecutor
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly ILogger<QueryExecutor> _logger;
        private readonly IQueryInterpreter _queryInterpreter;

        public QueryExecutor(ILoggerFactory loggerFactory, ICommandExecutor commandExecutor,
            IQueryInterpreter queryInterpreter)
        {
            _commandExecutor = commandExecutor;
            _queryInterpreter = queryInterpreter;
            _logger = loggerFactory.CreateLogger<QueryExecutor>();
        }

        public QueryResult ExecuteQuery(string queryPath, string scenario, int timeout)
        {
            var query = queryPath.Split("/").Last().Split('.').First();
            _logger.LogInformation("Executing : {}-{}", scenario, query);
            
            var task = _commandExecutor.ExecuteCommand(queryPath, _queryInterpreter.InitialScanPredicate());
            if (task.Wait(timeout))
            {
                var cmdResult = task.Result;
                var interpretedResult = _queryInterpreter.InterpretCommandResult(cmdResult);
                if (interpretedResult.IsError)
                {
                    _logger.LogError("Process errored: {}", interpretedResult.ErrorMessage);
                    return new QueryResult(0, 0, query, scenario, "Error occured - see logs");
                }

                _logger.LogInformation("{}-{} - Execution time: {}, Planning time: {}", scenario, query,
                    interpretedResult.ExecutionTime,
                    interpretedResult.PlanningTime);
                return new QueryResult(interpretedResult.PlanningTime, interpretedResult.ExecutionTime, query,
                    scenario);
            }

            _logger.LogWarning("Command timout");
            return new QueryResult(0, 0, query, scenario, $"Timeout at {timeout}ms");
        }
    }
}