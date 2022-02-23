using System;
using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Implementations;
using AutoDbPerf.Implementations.BigQuery;
using AutoDbPerf.Implementations.Postgres;
using AutoDbPerf.Records;
using NUnit.Framework;

namespace test_auto_db_perf
{
    public class TestAggregators
    {
        private Dictionary<Data, float> NumDict(List<Data> data)
        {
            var random = new Random();
            return data.ToDictionary(x => x, x => (float)random.NextDouble());
        }

        private Dictionary<Data, string> StrDict(List<Data> data)
        {
            return data.ToDictionary(x => x, x => "");
        }

        [Test]
        public void BasicAggregator_WillReturnCorrectKeys()
        {
            var queryResults = new List<QueryResult>
            {
                new("scenario1", "query1", NumDict(new List<Data> { Data.EXECUTION_TIME }), null),
                new("scenario1", "query1", NumDict(new List<Data> { Data.EXECUTION_TIME }), null),
                new("scenario1", "query1", NumDict(new List<Data> { Data.EXECUTION_TIME }), null)
            };

            var resultAggregator = new BasicQueryResultAggregator();
            var result = resultAggregator.GetTableDataFrom(queryResults);

            Assert.That(result.NumericData.Keys.ToList(),
                Is.EquivalentTo(new List<Data> { Data.AVG_EXECUTION_TIME, Data.EXECUTION_STD_DEV }));
        }

        [Test]
        public void PgAggregator_WillReturnCorrectKeys()
        {
            var queryResults = new List<QueryResult>
            {
                new("scenario1", "query1", NumDict(new List<Data> { Data.EXECUTION_TIME, Data.PLANNING_TIME }), null),
                new("scenario1", "query1", NumDict(new List<Data> { Data.EXECUTION_TIME, Data.PLANNING_TIME }), null),
                new("scenario1", "query1", NumDict(new List<Data> { Data.EXECUTION_TIME, Data.PLANNING_TIME }), null)
            };

            var resultAggregator = new PgQueryResultAggregator();
            var result = resultAggregator.GetTableDataFrom(queryResults);

            Assert.That(result.NumericData.Keys.ToList(),
                Is.EquivalentTo(new List<Data>
                {
                    Data.AVG_EXECUTION_TIME, Data.EXECUTION_STD_DEV, Data.AVG_PLANNING_TIME, Data.PLANNING_STD_DEV
                }));
        }

        [Test]
        public void BqAggregator_WillReturnCorrectKeys()
        {
            var queryResults = new List<QueryResult>
            {
                new("scenario1", "query1",
                    NumDict(new List<Data>
                        { Data.EXECUTION_TIME, Data.BYTES_PROCESSED, Data.BYTES_BILLED }),
                    StrDict(new List<Data> { Data.BI_MODE })),
                new("scenario1", "query1",
                    NumDict(new List<Data>
                        { Data.EXECUTION_TIME, Data.BYTES_PROCESSED, Data.BYTES_BILLED }),
                    StrDict(new List<Data> { Data.BI_MODE })),
                new("scenario1", "query1",
                    NumDict(new List<Data>
                        { Data.EXECUTION_TIME, Data.BYTES_PROCESSED, Data.BYTES_BILLED }),
                    StrDict(new List<Data> { Data.BI_MODE }))
            };

            var resultAggregator = new BqQueryResultAggregator();
            var result = resultAggregator.GetTableDataFrom(queryResults);

            Assert.That(result.NumericData.Keys.ToList(),
                Is.EquivalentTo(new List<Data>
                {
                    Data.AVG_BYTES_BILLED,
                    Data.AVG_BYTES_PROCESSED,
                    Data.AVG_EXECUTION_TIME, 
                    Data.BYTES_BILLED_STD_DEV,
                    Data.BYTES_PROCESSED_STD_DEV,
                    Data.EXECUTION_STD_DEV,
                }));

            Assert.That(result.StringData.Keys.ToList(),
                Is.EquivalentTo(new List<Data>
                {
                    Data.BI_MODE
                }));
        }
    }
}