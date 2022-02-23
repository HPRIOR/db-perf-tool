using System.Collections.Generic;
using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface IColumnOrderer
    {
        List<string> GetOrderedColumns(TableData tableData);

    }
}