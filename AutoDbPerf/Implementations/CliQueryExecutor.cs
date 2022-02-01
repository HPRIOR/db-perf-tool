using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;
using Microsoft.Extensions.Logging;

namespace AutoDbPerf.Implementations
{
    public class CliQueryExecutor : IQueryExecutor
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly ILogger<CliQueryExecutor> _logger;
        private readonly IQueryInterpreter _queryInterpreter;

        public CliQueryExecutor(ILoggerFactory loggerFactory, ICommandExecutor commandExecutor,
            IQueryInterpreter queryInterpreter)
        {
            _commandExecutor = commandExecutor;
            _queryInterpreter = queryInterpreter;
            _logger = loggerFactory.CreateLogger<CliQueryExecutor>();
        }

        public QueryResult ExecuteQuery(string queryPath, string scenario, int timeout)
        {
            var queryName = queryPath.GetQueryNameFromPath();
            _logger.LogInformation("Executing : {}-{}", scenario, queryName);
            
            var task = _commandExecutor.ExecuteCommand(queryPath, _queryInterpreter.InitialScanPredicate());
            if (task.Wait(timeout))
            {
                var cmdResult = task.Result;
                var interpretedResult = _queryInterpreter.InterpretCommandResult(cmdResult);
                if (interpretedResult.IsError)
                {
                    _logger.LogError("Process errored: {}", interpretedResult.ErrorMessage);
                    return new QueryResult(0, 0, queryName, scenario, "Error occured - see logs");
                }

                _logger.LogInformation("{}-{} - Execution time: {}, Planning time: {}", scenario, queryName,
                    interpretedResult.ExecutionTime,
                    interpretedResult.PlanningTime);
                return new QueryResult(interpretedResult.PlanningTime, interpretedResult.ExecutionTime, queryName,
                    scenario);
            }

            _logger.LogWarning("Command timout");
            return new QueryResult(0, 0, queryName, scenario, $"Timeout at {timeout}ms");
        }
    }
}