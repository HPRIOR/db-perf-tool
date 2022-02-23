using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Implementations;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;
using NUnit.Framework;

namespace test_auto_db_perf
{
    using TableResults = Dictionary<(string scenario, string query), TableResult>;

    public class TestColumnOrderer
    {
        private IColumnOrderer _columnOrderer = new ColumnOrderer();

        private TableResults GetTableResult(List<Data>? numData, List<Data>? strData)
        {
            var numDict = (numData ?? new List<Data>()).Any() ? numData?.ToDictionary(x => x, x => 1f) : null;
            var strDict = (strData ?? new List<Data>()).Any() ? strData?.ToDictionary(x => x, x => "") : null;
            return new Dictionary<(string scenario, string query), TableResult>
            {
                { ("query1", "query2"), new TableResult(numDict, strDict) }
            };
        }

        [Test]
        public void NoData_WillReturnEmptyList()
        {
            var tableResults = GetTableResult(null, null);
            var sut = _columnOrderer.GetOrderedColumns(new TableData(tableResults));
            Assert.That(sut.Count == 0);
        }

        [Test]
        public void SingleNumData_WillReturnSingleColumn()
        {
            var tableResults = GetTableResult(new List<Data> { Data.EXECUTION_TIME }, null);
            var sut = _columnOrderer.GetOrderedColumns(new TableData(tableResults));
            Assert.That(sut.First(), Is.EqualTo(Data.EXECUTION_TIME.AsString()));
        }

        [Test]
        public void MultipleNumData_WillReturnMultipleColumns()
        {
            var tableResults =
                GetTableResult(new List<Data> { Data.EXECUTION_TIME, Data.PLANNING_TIME, Data.BYTES_BILLED }, null);
            var sut = _columnOrderer.GetOrderedColumns(new TableData(tableResults));
            Assert.That(sut.Count == 3);
        }

        [Test]
        public void MultipleNumData_WillReturnOrderedColumns()
        {
            var tableResults =
                GetTableResult(new List<Data> { Data.PLANNING_TIME, Data.EXECUTION_TIME, Data.BYTES_BILLED }, null);
            var sut = _columnOrderer.GetOrderedColumns(new TableData(tableResults));
            Assert.That(sut[0], Is.EqualTo(Data.EXECUTION_TIME.AsString()));
            Assert.That(sut[1], Is.EqualTo(Data.PLANNING_TIME.AsString()));
            Assert.That(sut[2], Is.EqualTo(Data.BYTES_BILLED.AsString()));
        }

        [Test]
        public void SingleStrData_WillReturnColumn()
        {
            var tableResults = GetTableResult(null, new List<Data> { Data.BI_MODE });
            var sut = _columnOrderer.GetOrderedColumns(new TableData(tableResults));
            Assert.That(sut.First(), Is.EqualTo(Data.BI_MODE.AsString()));
        }

        [Test]
        public void MultipleStrData_WillReturnMultipleColumns()
        {
            var tableResults = GetTableResult(null, new List<Data> { Data.BI_MODE, Data.BYTES_BILLED });
            var sut = _columnOrderer.GetOrderedColumns(new TableData(tableResults));
            Assert.That(sut.Count == 2);
        }

        [Test]
        public void MultipleStrData_WillReturnOrderedColumns()
        {
            var tableResults =
                GetTableResult(null, new List<Data> { Data.BI_MODE, Data.BYTES_BILLED, Data.PLANNING_TIME });
            var sut = _columnOrderer.GetOrderedColumns(new TableData(tableResults));
            Assert.That(sut[0], Is.EqualTo(Data.PLANNING_TIME.AsString()));
            Assert.That(sut[1], Is.EqualTo(Data.BYTES_BILLED.AsString()));
            Assert.That(sut[2], Is.EqualTo(Data.BI_MODE.AsString()));
        }

        [Test]
        public void MixedData_WillReturnSumOfBothData()
        {
            var tableResults = GetTableResult(
                new List<Data> { Data.PLANNING_TIME, Data.AVG_BYTES_BILLED },
                new List<Data> { Data.BI_MODE, Data.BYTES_BILLED }
            );
            var sut = _columnOrderer.GetOrderedColumns(new TableData(tableResults));
            Assert.That(sut.Count == 4);
        }

        [Test]
        public void MixedData_WillReturnOrderedColumns()
        {
            var tableResults = GetTableResult(
                new List<Data> { Data.PLANNING_TIME, Data.AVG_BYTES_BILLED },
                new List<Data> { Data.BI_MODE, Data.BYTES_BILLED }
            );
            var sut = _columnOrderer.GetOrderedColumns(new TableData(tableResults));
            Assert.That(sut[0], Is.EqualTo(Data.PLANNING_TIME.AsString()));
            Assert.That(sut[1],Is.EqualTo(Data.BYTES_BILLED.AsString()));
            Assert.That(sut[2], Is.EqualTo(Data.AVG_BYTES_BILLED.AsString()));
            Assert.That(sut[3], Is.EqualTo(Data.BI_MODE.AsString()));
        }
    }
}