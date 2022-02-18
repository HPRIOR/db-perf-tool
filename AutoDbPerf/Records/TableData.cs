using System.Collections.Generic;

namespace AutoDbPerf.Records
{
    public record TableData
    {
        public TableData(IEnumerable<string> columns, IEnumerable<string> rows,
            Dictionary<(string, string), TableResult2> rowColumnData)
        {
            Columns = columns;
            Rows = rows;
            RowColumnData = rowColumnData;
        }

        public IEnumerable<string> Columns { get; }
        public IEnumerable<string> Rows { get; }
        public Dictionary<(string scenario, string query), TableResult2> RowColumnData { get; }
    }
}