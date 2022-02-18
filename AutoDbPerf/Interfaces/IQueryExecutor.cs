using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface IQueryExecutor
    {
        QueryResult2 ExecuteQuery(string queryPath, string scenario, int timeout);
    }
}