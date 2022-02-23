using AutoDbPerf.Implementations;
using AutoDbPerf.Implementations.Elastic;
using AutoDbPerf.Implementations.Exceptions;
using AutoDbPerf.Interfaces;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class TestElasticCommandGenerator
    {
        private ICommandGenerator GetElasticCommandGenerator(string elasticIndex = "", string indexV = "",
            string host = "localhost") =>
            new ElasticCommandGenerator(new Context(elasticIndex, indexV, host));


        private class Context : IContext
        {
            private readonly string _elasticIndex;
            private readonly string _indexV;
            private readonly string _localHost;

            public Context(string elasticIndex = "", string indexV = "", string host = "localhost")
            {
                _elasticIndex = elasticIndex;
                _indexV = indexV;
                _localHost = host;
            }

            public string GetEnv(ContextKey contextKey)
            {
                return contextKey switch
                {
                    ContextKey.ELASTICINDEX => _elasticIndex,
                    ContextKey.INDEXV => _indexV,
                    ContextKey.HOST => _localHost,
                    _ => ""
                };
            }
        }

        [Test]
        public void WillReturnCorrectResultString()
        {
            var elasticCommandGenerator = GetElasticCommandGenerator("test_index");
            var sut = elasticCommandGenerator.GenerateCommand("Resources/elastic/scenario1/query1.json");
            var expected =
                "-c \"curl -XPOST --header 'Content-Type: application/json' \\\"http://localhost:9200/test_index/_search?request_cache=false&filter_path=took\\\" -d'{  \\\"query\\\": {    \\\"match_all\\\": {}  }}' --silent --show-error\"";

            Assert.That(sut, Is.EqualTo(expected));
        }


        [Test]
        public void WillThrowError_IfNoMatchingScenarioPath_OrIndexInEnv()
        {
            var elasticCommandGenerator = GetElasticCommandGenerator();
            Assert.Throws<ElasticIndexException>(() =>
                elasticCommandGenerator.GenerateCommand("Resources/elastic/scenario1/query1.json"));
        }

        [Test]
        public void WillUseGivenIndex_WithMatchingScenarioFolder()
        {
            var elasticCommandGenerator = GetElasticCommandGenerator("test_index");
            var sut = elasticCommandGenerator.GenerateCommand("Resources/elastic/baseline/query1.json");
            var expected =
                "-c \"curl -XPOST --header 'Content-Type: application/json' \\\"http://localhost:9200/test_index/_search?request_cache=false&filter_path=took\\\" -d'{  \\\"query\\\": {    \\\"match_all\\\": {}  }}' --silent --show-error\"";

            Assert.That(sut, Is.EqualTo(expected));
        }


        [Test]
        public void WillUseDictionaryIndex_IfNoIndexProvided()
        {
            var elasticCommandGenerator = GetElasticCommandGenerator(indexV: "data");
            var sut = elasticCommandGenerator.GenerateCommand("Resources/elastic/baseline/query1.json");

            var expected =
                "-c \"curl -XPOST --header 'Content-Type: application/json' \\\"http://localhost:9200/data_de_mediatablets/_search?request_cache=false&filter_path=took\\\" -d'{  \\\"query\\\": {    \\\"match_all\\\": {}  }}' --silent --show-error\"";

            Assert.That(sut, Is.EqualTo(expected));
        }

        [Test]
        public void WillUseHostProvided()
        {
            var elasticCommandGenerator = GetElasticCommandGenerator("test_index", host: "test_host");
            var sut = elasticCommandGenerator.GenerateCommand("Resources/elastic/baseline/query1.json");

            var expected =
                "-c \"curl -XPOST --header 'Content-Type: application/json' \\\"http://test_host:9200/test_index/_search?request_cache=false&filter_path=took\\\" -d'{  \\\"query\\\": {    \\\"match_all\\\": {}  }}' --silent --show-error\"";

            Assert.That(sut, Is.EqualTo(expected));
        }
    }
}