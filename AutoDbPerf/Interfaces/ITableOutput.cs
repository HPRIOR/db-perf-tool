using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface ITableOutput
    {
        string OutputResults(TableData tableData);
    }
}