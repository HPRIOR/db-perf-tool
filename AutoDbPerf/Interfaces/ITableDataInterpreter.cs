using AutoDbPerf.Records;

namespace AutoDbPerf.Interfaces
{
    public interface ITableDataInterpreter
    {
        string Interpret(TableResult2 tableResult);
    }
}