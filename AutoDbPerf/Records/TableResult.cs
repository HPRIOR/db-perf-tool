using System.Collections.Generic;
using AutoDbPerf.Implementations;

namespace AutoDbPerf.Records
{
    public record TableResult(
        Dictionary<Data, float> NumericData,
        Dictionary<Data, string> StringData,
        bool HasProblem = false,
        bool IsEmpty = false);
}