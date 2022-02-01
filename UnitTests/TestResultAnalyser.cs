using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoDbPerf.Implementations;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using NUnit.Framework;

namespace test_auto_db_perf
{
    [TestFixture]
    public class TestResultAnalyser
    {
        private class Context : IContext
        {
            private readonly bool _ignoreFirst;

            public Context(bool ignoreFirst)
            {
                _ignoreFirst = ignoreFirst;
            }

            public string GetEnv(ContextKey contextKey)
            {
                return contextKey switch
                {
                    ContextKey.IGNOREFIRST => _ignoreFirst.ToString().ToLower(),
                    _ => throw new ArgumentOutOfRangeException(nameof(contextKey), contextKey, null)
                };
            }
        }

        private IEnumerable<QueryResult> GetQueryResults() => new[]
        {
            new QueryResult(0.244f, 0.049f, "query1", "scenario1"),
            new QueryResult(0.208f, 0.044f, "query2", "scenario1"),
            new QueryResult(0.241f, 0.048f, "query3", "scenario2"),
            new QueryResult(0.200f, 0.042f, "query4", "scenario2"),
        };

        private IEnumerable<QueryResult> GetQueryResultsWithDuplicatedRows() => new[]
        {
            new QueryResult(0.100f, 0.100f, "query1", "scenario1"),
            new QueryResult(0.200f, 0.200f, "query1", "scenario1"),
            new QueryResult(0.208f, 0.044f, "query2", "scenario1"),
            new QueryResult(0.241f, 0.048f, "query3", "scenario2"),
            new QueryResult(0.200f, 0.300f, "query4", "scenario2"),
            new QueryResult(0.300f, 0.200f, "query4", "scenario2"),
        };

        private IEnumerable<QueryResult> GetQueryResultsWithSomeProblems() => new[]
        {
            new QueryResult(0.100f, 0.100f, "query1", "scenario1"),
            new QueryResult(0.200f, 0.200f, "query1", "scenario1", "error"),
            new QueryResult(0.208f, 0.044f, "query2", "scenario1"),
            new QueryResult(0.241f, 0.048f, "query3", "scenario2"),
            new QueryResult(0.200f, 0.250f, "query4", "scenario2"),
            new QueryResult(0.300f, 0.200f, "query4", "scenario2", "timout-10"),
        };

        private IEnumerable<QueryResult> GetQueryResultsWithExclusiveProblemRows() => new[]
        {
            new QueryResult(0.100f, 0.100f, "query1", "scenario1", "error"),
            new QueryResult(0.200f, 0.200f, "query1", "scenario1", "error"),
            new QueryResult(0.208f, 0.044f, "query2", "scenario1"),
            new QueryResult(0.241f, 0.048f, "query3", "scenario2"),
            new QueryResult(0.200f, 0.250f, "query4", "scenario2", "timeout-10"),
            new QueryResult(0.300f, 0.200f, "query4", "scenario2", "timeout-10"),
        };

        private IEnumerable<QueryResult> GetQueryResultsWithMultiProblems() => new[]
        {
            new QueryResult(0.100f, 0.100f, "query1", "scenario1", "error"),
            new QueryResult(0.200f, 0.200f, "query1", "scenario1", "error"),
            new QueryResult(0.208f, 0.044f, "query2", "scenario1"),
            new QueryResult(0.241f, 0.048f, "query3", "scenario2"),
            new QueryResult(0.200f, 0.250f, "query4", "scenario2", "timeout-10"),
            new QueryResult(0.300f, 0.200f, "query4", "scenario2", "error"),
        };

        private IEnumerable<QueryResult> GetQueryResultsWithProblemsInFirstTwoQueries() => new[]
        {
            new QueryResult(0.100f, 0.100f, "query1", "scenario1", "probem"),
            new QueryResult(0.200f, 0.200f, "query1", "scenario1", "problem"),
            new QueryResult(0.300f, 0.300f, "query1", "scenario1"),
            new QueryResult(0.700f, 0.300f, "query1", "scenario1"),
        };

        private IEnumerable<QueryResult> GetQueryResultsAllDuplicates() => new[]
        {
            new QueryResult(0.100f, 0.100f, "query1", "scenario1"),
            new QueryResult(0.200f, 0.200f, "query1", "scenario1"),
            new QueryResult(0.300f, 0.300f, "query1", "scenario1"),
            new QueryResult(0.700f, 0.700f, "query1", "scenario1"),
        };

        private IEnumerable<QueryResult> GetQueryResultsInOrder() =>
            Enumerable.Range(0, 2)
                .SelectMany(x => Enumerable.Range(0, 2)
                    .SelectMany(y =>
                    {
                        return Enumerable.Range(0, 2).Select(async z =>
                        {
                            await Task.Delay(1);
                            return new QueryResult(x * y + z + 1, x * y + z + 1, $"query{x + 1}", $"scenario{y + 1}");
                        });
                    })
                    .Select(task => task.Result)
                );

        [Test]
        public void ReturnsCorrectColumns()
        {
            var queryResults = GetQueryResults();
            var resultAnalyser = new ResultAnalyser(new Context(false));
            var sut = resultAnalyser.AnalyseResults(queryResults);
            var expected = new[] { "scenario1", "scenario2" };
            Assert.That(sut.Columns, Is.EquivalentTo(expected));
        }

        [Test]
        public void ReturnsCorrectRows()
        {
            var queryResults = GetQueryResults();
            var resultAnalyser = new ResultAnalyser(new Context(false));
            var sut = resultAnalyser.AnalyseResults(queryResults);
            var expected = new[] { "query1", "query2", "query3", "query4" };
            Assert.That(sut.Rows, Is.EquivalentTo(expected));
        }

        [Test]
        public void ReturnsCorrectDictionaryKeys()
        {
            var queryResults = GetQueryResults();
            var resultAnalyser = new ResultAnalyser(new Context(false));
            var sut = resultAnalyser.AnalyseResults(queryResults);
            var expected = new[]
                { ("scenario1", "query1"), ("scenario1", "query2"), ("scenario2", "query3"), ("scenario2", "query4") };
            Assert.That(sut.RowColumnData.Keys.ToList(), Is.EquivalentTo(expected));
        }

        [Test]
        public void ReturnsCorrectDictionaryValues()
        {
            var queryResults = GetQueryResults();
            var resultAnalyser = new ResultAnalyser(new Context(false));
            var sut = resultAnalyser.AnalyseResults(queryResults);
            var expected = new Dictionary<(string, string), TableResult>()
            {
                { ("scenario1", "query1"), new TableResult(0.244f, 0.049f) },
                { ("scenario1", "query2"), new TableResult(0.208f, 0.044f) },
                { ("scenario2", "query3"), new TableResult(0.241f, 0.048f) },
                { ("scenario2", "query4"), new TableResult(0.200f, 0.042f) },
            };
            Assert.That(sut.RowColumnData, Is.EquivalentTo(expected));
        }

        [Test]
        public void ReturnsCorrectDictionaryValues_WithAvg()
        {
            var queryResults = GetQueryResultsWithDuplicatedRows();
            var resultAnalyser = new ResultAnalyser(new Context(false));
            var sut = resultAnalyser.AnalyseResults(queryResults);
            var expected = new Dictionary<(string, string), TableResult>()
            {
                { ("scenario1", "query1"), new TableResult(0.150f, 0.150f, 0.05f, 0.05f) },
                { ("scenario1", "query2"), new TableResult(0.208f, 0.044f) },
                { ("scenario2", "query3"), new TableResult(0.241f, 0.048f) },
                { ("scenario2", "query4"), new TableResult(0.250f, 0.250f, 0.05f, 0.05f) },
            };
            Assert.That(sut.RowColumnData, Is.EquivalentTo(expected));
        }


        [Test]
        public void ProblemResults_AreIgnored()
        {
            var queryResults = GetQueryResultsWithSomeProblems();
            var resultAnalyser = new ResultAnalyser(new Context(false));
            var sut = resultAnalyser.AnalyseResults(queryResults);
            var expected = new Dictionary<(string, string), TableResult>()
            {
                { ("scenario1", "query1"), new TableResult(0.100f, 0.100f) },
                { ("scenario1", "query2"), new TableResult(0.208f, 0.044f) },
                { ("scenario2", "query3"), new TableResult(0.241f, 0.048f) },
                { ("scenario2", "query4"), new TableResult(0.200f, 0.250f) },
            };
            Assert.That(sut.RowColumnData, Is.EquivalentTo(expected));
        }

        [Test]
        public void RowsWithAllProblems_ProduceMessageInTableResult()
        {
            var queryResults = GetQueryResultsWithExclusiveProblemRows();
            var resultAnalyser = new ResultAnalyser(new Context(false));
            var sut = resultAnalyser.AnalyseResults(queryResults);
            var expected = new Dictionary<(string, string), TableResult>()
            {
                { ("scenario1", "query1"), new TableResult(0, 0, 0, 0, "error") },
                { ("scenario1", "query2"), new TableResult(0.208f, 0.044f) },
                { ("scenario2", "query3"), new TableResult(0.241f, 0.048f) },
                { ("scenario2", "query4"), new TableResult(0, 0, 0, 0, "timeout-10") },
            };
            Assert.That(sut.RowColumnData, Is.EquivalentTo(expected));
        }


        [Test]
        public void RowsWithMultipleProblems_ProduceMessages()
        {
            var queryResults = GetQueryResultsWithMultiProblems();
            var resultAnalyser = new ResultAnalyser(new Context(false));
            var sut = resultAnalyser.AnalyseResults(queryResults);
            var expected = new Dictionary<(string, string), TableResult>()
            {
                { ("scenario1", "query1"), new TableResult(0, 0, 0, 0, "error") },
                { ("scenario1", "query2"), new TableResult(0.208f, 0.044f) },
                { ("scenario2", "query3"), new TableResult(0.241f, 0.048f) },
                { ("scenario2", "query4"), new TableResult(0, 0, 0, 0, "timeout-10,error") },
            };
            Assert.That(sut.RowColumnData, Is.EquivalentTo(expected));
        }


        [Test]
        public void WillNotCountFirstResult_IfSetInEnv()
        {
            var queryResults = GetQueryResultsInOrder();
            var resultAnalyser = new ResultAnalyser(new Context(true));
            var sut = resultAnalyser.AnalyseResults(queryResults);
            var expected = new Dictionary<(string, string), TableResult>()
            {
                { ("scenario1", "query1"), new TableResult(2f, 2f) },
                { ("scenario1", "query2"), new TableResult(2f, 2f) },
                { ("scenario2", "query1"), new TableResult(2f, 2f) },
                { ("scenario2", "query2"), new TableResult(3f, 3f) },
            };
            Assert.That(sut.RowColumnData, Is.EquivalentTo(expected));
        }

        [Test]
        public void WillNotIncludeFirstTwoResults_IfUnsuccessful()
        {
            var queryResults = GetQueryResultsWithProblemsInFirstTwoQueries();
            var resultAnalyser = new ResultAnalyser(new Context(true));
            var sut = resultAnalyser.AnalyseResults(queryResults);
            var expected = new Dictionary<(string, string), TableResult>()
            {
                { ("scenario1", "query1"), new TableResult(0.700f, 0.300f) },
            };
            Assert.That(sut.RowColumnData, Is.EquivalentTo(expected));
        }

        [Test]
        public void WillGetCorrectStdDeviation_IgnoreFirstFalse()
        {
            var queryResults = GetQueryResultsAllDuplicates();
            var resultAnalyser = new ResultAnalyser(new Context(false));
            var sut = resultAnalyser.AnalyseResults(queryResults);
            var expectedExcDev = 0.23f;
            var expectedPlanDev = 0.23f;
            Assert.That(sut.RowColumnData[("scenario1", "query1")].ExecutionStdDev, Is.EqualTo(expectedExcDev));
            Assert.That(sut.RowColumnData[("scenario1", "query1")].ExecutionStdDev, Is.EqualTo(expectedPlanDev));
        }

        [Test]
        public void WillGetCorrectStdDeviation_IgnoreFirstTrue()
        {
            var queryResults = GetQueryResultsAllDuplicates();
            var resultAnalyser = new ResultAnalyser(new Context(true));
            var sut = resultAnalyser.AnalyseResults(queryResults);
            var expectedExcDev = 0.22f;
            var expectedPlanDev = 0.22f;
            Assert.That(sut.RowColumnData[("scenario1", "query1")].ExecutionStdDev, Is.EqualTo(expectedExcDev));
            Assert.That(sut.RowColumnData[("scenario1", "query1")].ExecutionStdDev, Is.EqualTo(expectedPlanDev));
        }
    }
}