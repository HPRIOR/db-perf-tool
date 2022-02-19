using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AutoDbPerf.Records
{
    /*
     * Tech specific fields should be included in a HashMap.
     * The rest can stay the same (Message: string, Problem: bool)
     */
    public record TableResult(
        Dictionary<string, float> NumericData,
        Dictionary<string, string> StringData, // Do this for the QueryResult2
        bool HasProblem = false,
        string Message = ""
    );
}