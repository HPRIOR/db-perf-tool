using AutoDbPerf.Implementations.Elastic;
using AutoDbPerf.Implementations.Postgres;
using NUnit.Framework;

namespace test_auto_db_perf
{
    [TestFixture]
    public class TestElasticQueryInterpreter
    {
        private ElasticQueryInterpreter? _postgresQueryInterpreter;

        [SetUp]
        public void Setup()
        {
            _postgresQueryInterpreter = new ElasticQueryInterpreter();
        }
        //TODO
        
    }
}