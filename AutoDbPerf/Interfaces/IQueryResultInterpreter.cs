using System.Collections.Generic;
using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface IQueryResultInterpreter
    {
        TableResult2 GetTableDataFrom(IEnumerable<QueryResult2> queryResult);
    }
}