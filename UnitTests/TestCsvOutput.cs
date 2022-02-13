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
        [Test]
        public void CorrectOutputIsGiven_WithNonApplicableResults()
        {
            var columnRowData = new Dictionary<(string, string), TableResult>()
            {
                { ("scenario1", "query1"), new TableResult(0.244f, 0.049f) },
                { ("scenario1", "query2"), new TableResult(0.208f, 0.044f) },
                { ("scenario2", "query3"), new TableResult(0.241f, 0.048f) },
                { ("scenario2", "query4"), new TableResult(0.200f, 0.042f) },
            };
            var columns = new[] { "scenario1", "scenario2" };
            var rows = new[] { "query1", "query2", "query3", "query4" };
            var tableData = new TableData(columns, rows, columnRowData);
            var csvOutput = new CsvOutput(new LoggerFactory());
            var result = csvOutput.OutputResults(tableData);
            var expected = "scenarios,scenario1,scenario2\n" +
                           "query1,Planning: 0.244 SD: 0 Execution: 0.049 SD: 0 Total: 0.293,N/A\n" +
                           "query2,Planning: 0.208 SD: 0 Execution: 0.044 SD: 0 Total: 0.252,N/A\n" +
                           "query3,N/A,Planning: 0.241 SD: 0 Execution: 0.048 SD: 0 Total: 0.289\n" +
                           "query4,N/A,Planning: 0.2 SD: 0 Execution: 0.042 SD: 0 Total: 0.242\n";
            Assert.That(result, Is.EqualTo(expected));
        }


        [Test]
        public void CorrectOutputIsGiven_WithMessagesInScenario()
        {
            var columnRowData = new Dictionary<(string, string), TableResult>()
            {
                { ("scenario1", "query1"), new TableResult(0, 0, 0, 0, "error") },
                { ("scenario1", "query2"), new TableResult(0.208f, 0.044f) },
                { ("scenario2", "query3"), new TableResult(0.241f, 0.048f) },
                { ("scenario2", "query4"), new TableResult(0, 0, 0, 0, "problem") },
            };
            var columns = new[] { "scenario1", "scenario2" };
            var rows = new[] { "query1", "query2", "query3", "query4" };
            var tableData = new TableData(columns, rows, columnRowData);
            var csvOutput = new CsvOutput(new LoggerFactory());
            var result = csvOutput.OutputResults(tableData);
            var expected = "scenarios,scenario1,scenario2\n" +
                           "query1,Error - see logs,N/A\n" +
                           "query2,Planning: 0.208 SD: 0 Execution: 0.044 SD: 0 Total: 0.252,N/A\n" +
                           "query3,N/A,Planning: 0.241 SD: 0 Execution: 0.048 SD: 0 Total: 0.289\n" +
                           "query4,N/A,Error - see logs\n";
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void CorrectOutputIsGiven_WithBytesProcessed()
        {
            var columnRowData = new Dictionary<(string, string), TableResult>()
            {
                { ("scenario1", "query1"), new TableResult(0.244f, 0.049f, 0, 0, "", 1) },
                { ("scenario1", "query2"), new TableResult(0.208f, 0.044f, 0 ,0, "", 1) },
                { ("scenario2", "query3"), new TableResult(0.241f, 0.048f) },
                { ("scenario2", "query4"), new TableResult(0.200f, 0.042f) },
            };
            var columns = new[] { "scenario1", "scenario2" };
            var rows = new[] { "query1", "query2", "query3", "query4" };
            var tableData = new TableData(columns, rows, columnRowData);
            var csvOutput = new CsvOutput(new LoggerFactory());
            var result = csvOutput.OutputResults(tableData);
            var expected = "scenarios,scenario1,scenario2\n" +
                           "query1,Planning: 0.244 SD: 0 Execution: 0.049 SD: 0 Total: 0.293,N/A\n" +
                           "query2,Planning: 0.208 SD: 0 Execution: 0.044 SD: 0 Total: 0.252,N/A\n" +
                           "query3,N/A,Planning: 0.241 SD: 0 Execution: 0.048 SD: 0 Total: 0.289\n" +
                           "query4,N/A,Planning: 0.2 SD: 0 Execution: 0.042 SD: 0 Total: 0.242\n";
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}