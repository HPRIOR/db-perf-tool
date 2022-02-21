using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Schema;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations
{
    public class ColumnOrderer : IColumnOrderer
    {
        private bool TableResultHasData(TableResult tableResult) =>
            tableResult.NumericData != null || tableResult.StringData != null;

        public List<string> GetOrderedColumns(TableData tableData)
        {
            var columns = tableData.ScenarioColumns;
            var rows = tableData.Rows;
            
            var (scenario, query) = columns
                .SelectMany(col => rows.Select(row => (col, row)))
                .FirstOrDefault(t => tableData.HasDataFor(t.col, t.row) &&
                                     TableResultHasData(tableData.GetTableResult(t.col, t.row)));

            if (scenario == "" || query == "")
                return new List<string>();

            var tableResult = tableData.GetTableResult(scenario, query);

            var numericData = tableResult?.NumericData?.Keys.ToList() ?? new List<Data>();
            var stringData = tableResult?.StringData?.Keys.ToList() ?? new List<Data>();

            var data = numericData.Concat(stringData);
            return GetOrderedEnums(data).Select(d => d.AsString()).ToList();

        }

        private static List<Data> GetOrderedEnums(IEnumerable<Data> data)
        {
            return data.OrderBy(d =>
                {
                    var index = DataUtils.GetFixedOrderedData().IndexOf(d);
                    return index == -1 ? int.MaxValue : index;
                }
            ).ToList();
        }
    }
}