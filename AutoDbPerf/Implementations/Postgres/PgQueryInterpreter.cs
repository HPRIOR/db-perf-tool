using System;
using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations.Postgres
{
    public class PgQueryInterpreter : IQueryInterpreter
    {
        private const string PlanningIdentifier = "Planning";
        private const string ExecutionIdentifier = "Execution";

        public Predicate<string> InitialScanPredicate()
        {
            return str => str.Contains(PlanningIdentifier) || str.Contains(ExecutionIdentifier);
        }

        public InterpretedCommand InterpretCommandResult(CommandResult cmdResult)
        {
            var stdErrPresent = cmdResult.Stderr.Any();
            var noStdErrOrOut = !cmdResult.Stdout.Any() && !stdErrPresent;
            if (stdErrPresent)
                return new InterpretedCommand(true, ErrorMessage: cmdResult.Stderr.FlattenToParagraph());
            if (noStdErrOrOut)
                return new InterpretedCommand(true, ErrorMessage: "Could not find correct information in stdout");

            var planningTime = cmdResult.Stdout.GetFirstNumberFromLineWith(PlanningIdentifier);
            var executionTime = cmdResult.Stdout.GetFirstNumberFromLineWith(ExecutionIdentifier);
            return new InterpretedCommand(false, executionTime, planningTime);
        }
    }
}