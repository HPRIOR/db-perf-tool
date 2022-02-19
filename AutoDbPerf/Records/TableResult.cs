using System.Collections.Generic;
using AutoDbPerf.Implementations;

namespace AutoDbPerf.Records
{
    /*
     * Tech specific fields should be included in a HashMap.
     * The rest can stay the same (Message: string, Problem: bool)
     */
    public record TableResult(
        Dictionary<Data, float> NumericData,
        Dictionary<Data, string> StringData,
        bool HasProblem = false,
        bool IsEmpty = false)
    {
        public int DataNum { get; } = NumericData.Count + StringData.Count;
    }
}