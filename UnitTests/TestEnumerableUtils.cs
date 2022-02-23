using System;
using System.Collections.Generic;
using AutoDbPerf.Implementations;
using AutoDbPerf.Records;
using AutoDbPerf.Utils;
using NUnit.Framework;

namespace test_auto_db_perf
{
    [TestFixture]
    public class TestEnumerableUtils
    {
        [Test]
        public void AggregateStrings_WillAggregateStringsWithoutSeparator()
        {
            var input = new List<string>()
            {
                "A", "B", "C", "D"
            };

            var expected = "ABCD";
            Assert.That(input.AggregateToString(), Is.EqualTo(expected));
        }

        [Test]
        public void AggregateStrings_WillAggregateStringsWithSeparator()
        {
            var input = new List<string>()
            {
                "A", "B", "C", "D"
            };

            var expected = "A-B-C-D";
            Assert.That(input.AggregateToString("-"), Is.EqualTo(expected));
        }

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
                new QueryResult("test-query", "test-scenario", new Dictionary<Data, float>(),
                    new Dictionary<Data, string>());

            var input = new List<QueryResult>
            {
                new("test-query-1", "test-scenario-1", null, null, true),
                new("test-query-2", "test-scenario-2", null, null, true),
                savedQueryResult
            };
            var expected = new List<QueryResult>();

            Assert.That(input.AllAfterFirstSuccessful(), Is.EquivalentTo(expected));
        }

        [Test]
        public void AllAfterFirstSuccessful_WillIgnoreFirstThreeOutOfFour_IfThirdSuccessful()
        {
            var savedQueryResult =
                new QueryResult("test-query", "test-scenario", new Dictionary<Data, float>(),
                    new Dictionary<Data, string>());

            var input = new List<QueryResult>
            {
                new("test-query-1", "test-scenario-1", null, null, true),
                new("test-query-2", "test-scenario-2", null, null, true),
                new("test-query-2", "test-scenario-2",
                    new Dictionary<Data, float>() { { Data.PLANNING_TIME, 10f }, { Data.EXECUTION_TIME, 10f } }, null),
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
                new("test-query-1", "test-scenario-1", null, null, true),
                new("test-query-2", "test-scenario-2", null, null, true),
                new("test-query-2", "test-scenario-2", null, null, true),
            };
            var expected = new List<QueryResult>();

            Assert.That(input.AllAfterFirstSuccessful(), Is.EquivalentTo(expected));
        }

        [Test]
        public void AllAfterFirstSuccessful_WillIgnoreFirstSuccessfulTest_AfterOneFailure()
        {
            var expectedOne = new QueryResult("test", "test",
                new Dictionary<Data, float> { { Data.PLANNING_TIME, 10f }, { Data.EXECUTION_TIME, 10f } }, null);
            var expectedTwo = new QueryResult("test", "test",
                new Dictionary<Data, float> { { Data.PLANNING_TIME, 10f }, { Data.EXECUTION_TIME, 10f } }, null);
            var input = new List<QueryResult>
            {
                new("test-query-1", "test-scenario-1", null, null, true),
                expectedOne,
                expectedTwo
            };
            var expected = new List<QueryResult>
            {
                expectedTwo
            };

            Assert.That(input.AllAfterFirstSuccessful(), Is.EquivalentTo(expected));
        }

        [Test]
        public void StdDev_WillReturnCorrectResult()
        {
            var sut = new List<float> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }.StdDev();
            Assert.That(sut, Is.EqualTo((float)Math.Round(2.872281323269, 2)));
        }

        [Test]
        public void StdDev_WillReturnCorrectResult_WithSingleItem()
        {
            var sut = new List<float> { 1 }.StdDev();
            Assert.That(sut, Is.EqualTo(0));
        }

        [Test]
        public void StdDev_WillReturnCorrectResult_WithNoItems()
        {
            var sut = new List<float> { 0 }.StdDev();
            Assert.That(sut, Is.EqualTo(0));
        }

        [Test]
        public void Enumerate_WillEnumerate_MultipleItems()
        {
            var input = new List<string> { "1", "2", "3", "4" };
            var expected = new List<(int, string)>
            {
                (0, "1"),
                (1, "2"),
                (2, "3"),
                (3, "4"),
            };
            Assert.That(input.Enumerate(), Is.EquivalentTo(expected));
        }

        [Test]
        public void Enumerate_WillEnumerate_SingleItem()
        {
            var input = new List<string> { "1" };
            var expected = new List<(int, string)>
            {
                (0, "1"),
            };
            Assert.That(input.Enumerate(), Is.EquivalentTo(expected));
        }

        [Test]
        public void Enumerate_WillEnumerate_NoItems()
        {
            var input = new List<string>();
            var expected = new List<(int, string)>
            {
            };
            Assert.That(input.Enumerate(), Is.EquivalentTo(expected));
        }
    }
}