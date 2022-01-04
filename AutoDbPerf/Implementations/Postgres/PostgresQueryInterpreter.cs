using System;
using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations.Postgres
{
    public class PostgresQueryInterpreter : IQueryInterpreter
    {
        private const string PlanningIdentifier = "Planning";
        private const string ExecutionIdentifier = "Execution";

        public Predicate<string> InitialScanPredicate()
        {
            return str => str.Contains(PlanningIdentifier) || str.Contains(ExecutionIdentifier);
        }

        public InterpretedCommand InterpretCommandResult(CommandResult cmdResult)
        {
            if (cmdResult.Stderr.Any())
                return new InterpretedCommand(true, errorMessage: cmdResult.Stderr.FlattenToParagraph());

            var planningTime = cmdResult.Stdout.GetFirstNumberFromLineWith(PlanningIdentifier);
            var executionTime = cmdResult.Stdout.GetFirstNumberFromLineWith(ExecutionIdentifier);
            return new InterpretedCommand(false, executionTime, planningTime);
        }
    }
}