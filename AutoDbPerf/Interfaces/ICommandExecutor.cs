using System;
using System.Threading.Tasks;
using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface ICommandExecutor
    {
        public Task<CommandResult> ExecuteCommand(string queryPath, Predicate<string> shouldCapture);
    }
}