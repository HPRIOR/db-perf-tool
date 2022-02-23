using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;

namespace AutoDbPerf.Implementations
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly ICommandGenerator _commandGenerator;

        public CommandExecutor(ICommandGenerator commandGenerator)
        {
            _commandGenerator = commandGenerator;
        }

        public Task<CommandResult> ExecuteCommand(string queryPath, Predicate<string> shouldCapture)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = _commandGenerator.GenerateCommand(queryPath),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);

            return Task.FromResult(GetProcessOutput(process, shouldCapture));
        }

        private CommandResult GetProcessOutput(Process process, Predicate<string> shouldCaptureStdOutIf)
        {
            var stdout = process?.StandardOutput;
            var stderr = process?.StandardError;

            var stdoutResult =
                GetFromStream(stdout, shouldCaptureStdOutIf);
            var stderrResult =
                GetFromStream(stderr, _ => true); // get all lines 

            var processTask = process?.WaitForExitAsync();
            Task.WaitAll(stdoutResult, stderrResult, processTask);

            return new CommandResult(
                stdoutResult.Result,
                stderrResult.Result);
        }

        /// <summary>
        ///     Will retrieve only relevant lines from the <paramref name="stream" /> given the
        ///     <paramref name="retrieveIfPredicate" /> argument
        /// </summary>
        /// <remarks>
        ///     The initial 'pruning' done by the predicate is useful in instances where not enough memory is available
        ///     in the host machine to store all of stdout (e.g. a low powered VM). As the method is asynchronously only the
        ///     relevant parts of
        ///     the stream will reside in memory.
        /// </remarks>
        /// <param name="stream"></param>
        /// <param name="retrieveIfPredicate"></param>
        /// <returns></returns>
        private async Task<List<string>> GetFromStream(StreamReader stream, Predicate<string> retrieveIfPredicate)
        {
            var result = new List<string>();
            string stdout;
            while ((stdout = await stream?.ReadLineAsync()) != null)
                if (retrieveIfPredicate(stdout))
                    result.Add(stdout);

            return result;
        }
    }
}