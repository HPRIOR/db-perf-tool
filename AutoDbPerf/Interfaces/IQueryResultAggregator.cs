using System;
using System.Collections.Generic;
using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface IQueryResultAggregator
    {
        TableResult GetTableDataFrom(IEnumerable<QueryResult> queryResult);
    }
}