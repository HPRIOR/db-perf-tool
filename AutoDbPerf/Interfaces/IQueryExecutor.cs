using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface IQueryExecutor
    {
        QueryResult ExecuteQuery(string queryPath, string scenario, int timeout);
    }
}