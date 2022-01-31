using System.Collections.Generic;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;
using NSubstitute.Core;
using NUnit.Framework;

namespace test_auto_db_perf
{
    [TestFixture]
    public class TestEnumerableUtils
    {
        [Test]
        public void FlattenToParagraph_WillWorkOnSingleItemList()
        {
            var input = new List<string>()
            {
                "Line1",
            };

            var expected = "Line1";
            Assert.That(input.FlattenToParagraph(), Is.EqualTo(expected));
        }

        [Test]
        public void FlattenToParagraph_WillProduceStringsWithLineBreaks()
        {
            var input = new List<string>()
            {
                "Line1",
                "Line2",
                "Line3",
                "Line4",
            };

            var expected = "Line1\nLine2\nLine3\nLine4";
            Assert.That(input.FlattenToParagraph(), Is.EqualTo(expected));
        }

        [Test]
        public void FlattenToCommmaList_WillProduceCommandSeparatedWords()
        {
            var input = new List<string>()
            {
                "Line1",
                "Line2",
                "Line3",
                "Line4",
            };

            var expected = "Line1,Line2,Line3,Line4";
            Assert.That(input.FlattenToCommaList(), Is.EqualTo(expected));
        }

        [Test]
        public void GetFirstNumberFromLineWith_WillGetTimeFromSpaceSeperatedLine()
        {
            var input = new List<string>()
            {
                "hello the time is 10 o'clock",
                "this does not contain the identifier"
            };

            var expected = 10f;
            Assert.That(input.GetFirstNumberFromLineWith("hello"), Is.EqualTo(expected));
        }


        [Test]
        public void GetFirstNumberFromLineWith_WillGetFirstNumberOfMany()
        {
            var input = new List<string>()
            {
                "hello the time is 10 1 2 3 4 o'clock",
                "this does not contain the identifier"
            };

            var expected = 10f;
            Assert.That(input.GetFirstNumberFromLineWith("hello"), Is.EqualTo(expected));
        }

        [Test]
        public void GetFirstNumberFromLineWithoutSpaces_WillRetrieveElasticTook()
        {
            var input = new List<string>()
            {
                "{\"took\":2}",
            };

            var expected = 2;
            Assert.That(input.GetNumberFromLineWithoutSpaces("took"), Is.EqualTo(expected));
        }

        [Test]
        public void GetFirstNumberFromLineWith_WillGetFirstFromManyLines()
        {
            var input = new List<string>()
            {
                "hello the time is 10 o'clock",
                "this does not contain the identifier",
                "1 this does not contain the identifier",
                "2 this does not contain the identifier",
                "3 this does not contain the identifier",
            };

            var expected = 10f;
            Assert.That(input.GetFirstNumberFromLineWith("hello"), Is.EqualTo(expected));
        }

        [Test]
        public void AllButFirst_WillRemoveFirstElement()
        {
            var input = new List<string>
            {
                "first",
                "second",
                "third"
            };
            var expected = new List<string>
            {
                "second",
                "third"
            };

            Assert.That(input.AllButFirst(), Is.EquivalentTo(expected));
        }

        [Test]
        public void AllAfterFirstSuccessful_WillIgnoreAll_IfOnlyLastIsSuccessful()
        {
            var savedQueryResult =
                new QueryResult(1, 1, "test-query", "test-scenario");

            var input = new List<QueryResult>
            {
                new(0, 0, "test-query-1", "test-scenario-1", "some problem 1"),
                new(0, 0, "test-query-2", "test-scenario-2", "some problem 2"),
                savedQueryResult
            };
            var expected = new List<QueryResult>();

            Assert.That(input.AllAfterFirstSuccessful(), Is.EquivalentTo(expected));
        }

        [Test]
        public void AllAfterFirstSuccessful_WillIgnoreFirstThreeOutOfFour_IfThirdSuccessful()
        {
            var savedQueryResult =
                new QueryResult(1, 1, "test-query", "test-scenario");

            var input = new List<QueryResult>
            {
                new(0, 0, "test-query-1", "test-scenario-1", "some problem 1"),
                new(0, 0, "test-query-2", "test-scenario-2", "some problem 2"),
                new(10, 10, "test-query-2", "test-scenario-2"),
                savedQueryResult
            };
            var expected = new List<QueryResult> { savedQueryResult };

            Assert.That(input.AllAfterFirstSuccessful(), Is.EquivalentTo(expected));
        }

        [Test]
        public void AllAfterFirstSuccessful_WillIgnoreAllTests_IfAllUnsuccessful()
        {
            var input = new List<QueryResult>
            {
                new(0, 0, "test-query-1", "test-scenario-1", "some problem 1"),
                new(0, 0, "test-query-2", "test-scenario-2", "some problem 2"),
                new(0, 0, "test-query-2", "test-scenario-2", "some problem 2"),
            };
            var expected = new List<QueryResult>();

            Assert.That(input.AllAfterFirstSuccessful(), Is.EquivalentTo(expected));
        }

        [Test]
        public void AllAfterFirstSuccessful_WillIgnoreFirstSuccessfulTest_AfterOneFailure()
        {
            var expectedOne = new QueryResult(1, 1, "test", "test");
            var expectedTwo = new QueryResult(1, 1, "test", "test");
            var input = new List<QueryResult>
            {
                new(0, 0, "test-query-1", "test-scenario-1", "some problem 1"),
                expectedOne,
                expectedTwo
            };
            var expected = new List<QueryResult>
            {
                expectedTwo
            };

            Assert.That(input.AllAfterFirstSuccessful(), Is.EquivalentTo(expected));
        }
    }
}