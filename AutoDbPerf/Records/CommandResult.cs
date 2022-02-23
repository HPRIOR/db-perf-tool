using System.Collections.Generic;

namespace AutoDbPerf.Records
{
    public record CommandResult(IEnumerable<string> Stdout, IEnumerable<string> Stderr);
}