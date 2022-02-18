using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AutoDbPerf.Records
{
    /*
     * Tech specific fields should be included in a HashMap.
     * The rest can stay the same (Message: string, Problem: bool)
     */
    public record TableResult2(
        Dictionary<string, float> Data,
        bool HasProblem = false,
        string Message = ""
    );
}