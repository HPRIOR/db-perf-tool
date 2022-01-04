using System;
using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface IQueryInterpreter
    {
        Predicate<string> InitialScanPredicate();
        InterpretedCommand InterpretCommandResult(CommandResult cmdResult);
    }
}