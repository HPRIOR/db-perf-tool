using System;
using System.Linq;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations.Elastic
{
    public class ElasticQueryInterpreter : IQueryInterpreter
    {
        private const string TookIdentifier = "took";
        private const string ErrorIdentifier = "error";

        public Predicate<string> InitialScanPredicate()
        {
            return str => str.Contains(TookIdentifier) || str.Contains(ErrorIdentifier);
        }

        public InterpretedCommand InterpretCommandResult(CommandResult cmdResult)
        {
            // Curl does an annoying thing where it writes network information to stderr
            if (StdErrContainsRealErrorMessage(cmdResult))
                return new InterpretedCommand(true, ErrorMessage: cmdResult.Stderr.FlattenToParagraph());

            if (StdOutContainsError(cmdResult))
                return new InterpretedCommand(true, ErrorMessage: cmdResult.Stdout.FlattenToParagraph());

            var time = cmdResult.Stdout.GetNumberFromLineWithoutSpaces(TookIdentifier);
            return new InterpretedCommand(false, time);
        }

        private bool StdOutContainsError(CommandResult cmdResult)
        {
            return cmdResult.Stdout.Any(str => str.Contains(ErrorIdentifier));
        }

        private bool StdErrContainsRealErrorMessage(CommandResult cmdResult) =>
            cmdResult.Stderr.Any() && !cmdResult.Stderr.First().Contains("% Total");
    }
}