using System;
using System.Collections.Generic;
using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface IQueryResultInterpreter
    {
        TableResult GetTableDataFrom(IEnumerable<QueryResult> queryResult);
    }
}