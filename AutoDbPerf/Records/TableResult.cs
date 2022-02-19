using System.Collections.Generic;

namespace AutoDbPerf.Records
{
    /*
     * Tech specific fields should be included in a HashMap.
     * The rest can stay the same (Message: string, Problem: bool)
     */
    public record TableResult(
        Dictionary<string, float> NumericData,
        Dictionary<string, string> StringData,
        bool HasProblem = false,
        bool IsEmpty = false)
    {
        public int DataNum { get; } = NumericData.Count + StringData.Count;
    }
}