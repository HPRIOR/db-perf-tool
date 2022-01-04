using System.Collections.Generic;

namespace AutoDbPerf.Records
{
    public record CommandResult
    {
        public CommandResult(IEnumerable<string> stdout, IEnumerable<string> stderr)
        {
            Stdout = stdout;
            Stderr = stderr;
        }

        public IEnumerable<string> Stdout { get; }
        public IEnumerable<string> Stderr { get; }
    }
}