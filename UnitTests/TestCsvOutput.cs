using System.Collections.Generic;
using AutoDbPerf.Implementations;
using AutoDbPerf.Records;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace test_auto_db_perf
{
    [TestFixture]
    public class TestCsvOutput
    {
        private Dictionary<Data, float> CreateDataDict(float executionTime, float planningTime) =>
            new()
            {
                { Data.EXECUTION_TIME, executionTime },
                { Data.PLANNING_TIME, planningTime }
            };


        //TODO further test relationship between cols,rows, orderedData
        //TODO Mock Column orderer and test that in isolation

        [Test]
        public void CorrectOutputIsGiven_WithMessagesInScenario()
        {
            var columnRowData = new Dictionary<(string, string), TableResult>
            {
                { ("scenario1", "query1"), new TableResult(CreateDataDict(0, 0), null, true) },
                { ("scenario1", "query2"), new TableResult(CreateDataDict(0.208f, 0.044f), null) },
                { ("scenario2", "query3"), new TableResult(CreateDataDict(0.241f, 0.048f), null) },
                { ("scenario2", "query4"), new TableResult(CreateDataDict(0, 0), null, true) }
            };

            var tableData = new TableData(columnRowData);
            var csvOutput = new CsvOutput(new LoggerFactory(), new ColumnOrderer());
            var result = csvOutput.OutputResults(tableData);
            var expected = "scenarios,scenario1,,scenario2\n" +
                           ",ExecutionTime,PlanningTime,ExecutionTime,PlanningTime\n" +
                           "query1,Error - see logs,Error - see logs,N/A,N/A\n" +
                           "query2,0.208,0.044,N/A,N/A\n" +
                           "query3,N/A,N/A,0.241,0.048\n" +
                           "query4,N/A,N/A,Error - see logs,Error - see logs\n";
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void CorrectOutputIsGiven_WithBiMode()
        {
            var columnRowData = new Dictionary<(string, string), TableResult>
            {
                {
                    ("scenario1", "query1"),
                    new TableResult(CreateDataDict(0.244f, 0.049f),
                        new Dictionary<Data, string> { { Data.BI_MODE, "FULL" } })
                },
                {
                    ("scenario2", "query2"),
                    new TableResult(CreateDataDict(0.208f, 0.044f),
                        new Dictionary<Data, string> { { Data.BI_MODE, "FULL" } })
                }
            };
            var tableData = new TableData(columnRowData);
            var csvOutput = new CsvOutput(new LoggerFactory(), new ColumnOrderer());
            var result = csvOutput.OutputResults(tableData);
            var expected = "scenarios,scenario1,,,scenario2\n" +
                           ",ExecutionTime,PlanningTime,BiEngineMode,ExecutionTime,PlanningTime,BiEngineMode\n" +
                           "query1,0.244,0.049,FULL,N/A,N/A,N/A\n" +
                           "query2,N/A,N/A,N/A,0.208,0.044,FULL\n";
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}