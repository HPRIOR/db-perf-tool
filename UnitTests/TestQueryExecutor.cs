using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoDbPerf.Implementations;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Records;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace test_auto_db_perf
{
    [TestFixture]
    public class TestQueryExecutor
    {
        [SetUp]
        public void Setup()
        {
            bool InitialScanPredicate(string _) => true;

            // Mock ICommandExecutor
            var commandExecutor = Substitute.For<ICommandExecutor>();
            var errorCommandResult = new CommandResult(new[] { "" }, new[] { "Error message", "With new line" });
            commandExecutor.ExecuteCommand("path/to/error.sql", InitialScanPredicate)
                .Returns(Task.FromResult(errorCommandResult));

            var planningCommandResult = new CommandResult(new[] { "Planning time: 10" }, new List<string>());
            commandExecutor.ExecuteCommand("not error", InitialScanPredicate)
                .Returns(planningCommandResult);
            commandExecutor.ExecuteCommand("planning", InitialScanPredicate)
                .Returns(planningCommandResult);

            var executionCommandResult = new CommandResult(new[] { "Execution time: 10" }, new List<string>());
            commandExecutor.ExecuteCommand("execution", InitialScanPredicate)
                .Returns(executionCommandResult);

            var executionNoTimeResult = new CommandResult(new[] { "Execution time: " }, new List<string>());
            commandExecutor.ExecuteCommand("no-num", InitialScanPredicate)
                .Returns(executionNoTimeResult);

            var timeoutResult =
                Task<Task<CommandResult>>.Factory.StartNew(async () =>
                {
                    await Task.Delay(50);
                    return new CommandResult(new[] { "" }, new[] { "" });
                });
            commandExecutor.ExecuteCommand("timeout", InitialScanPredicate).Returns(timeoutResult.Result);

            // Mock IQueryInterpreter
            var queryInterpreter = Substitute.For<IQueryInterpreter>();
            queryInterpreter.InitialScanPredicate().Returns(InitialScanPredicate);
            queryInterpreter.InterpretCommandResult(errorCommandResult)
                .Returns(new InterpretedCommand(true , 0, 0,"Error occured - see logs"));
            queryInterpreter.InterpretCommandResult(planningCommandResult)
                .Returns(new InterpretedCommand(false, 0, 10));
            queryInterpreter.InterpretCommandResult(planningCommandResult)
                .Returns(new InterpretedCommand(false, 0, 10));
            queryInterpreter.InterpretCommandResult(executionCommandResult)
                .Returns(new InterpretedCommand(false , 10));
            queryInterpreter.InterpretCommandResult(executionNoTimeResult)
                .Returns(new InterpretedCommand(false));

            _queryExecutor = new CliQueryExecutor(new LoggerFactory(), commandExecutor, queryInterpreter);
        }

        private IQueryExecutor? _queryExecutor;


        [Test]
        public void WillReturnStdError_IfReturnedFromCommand()
        {
            var sut = _queryExecutor?.ExecuteQuery("path/to/error.sql", "scenario", 100);
            Assert.That(sut?.Problem, Is.EqualTo("Error occured - see logs"));
        }


        [Test]
        public void WillNotReturnStdError_IfNotReturnedFromCommand()
        {
            var sut = _queryExecutor?.ExecuteQuery("not error", "scenario", 100);
            Assert.That(sut?.Problem, Is.EqualTo(string.Empty));
        }

        [Test]
        public void WillGetCorrectPlanningTime()
        {
            var sut = _queryExecutor?.ExecuteQuery("planning", "scenario", 100);
            Assert.That(sut?.NumData[Data.PLANNING_TIME], Is.EqualTo(10));
        }

        [Test]
        public void WillGetCorrectExecutionTime()
        {
            var sut = _queryExecutor?.ExecuteQuery("execution", "scenario", 100);
            Assert.That(sut?.NumData[Data.EXECUTION_TIME], Is.EqualTo(10));
        }

        [Test]
        public void WillReturnZeroTimes_WhenNoNumbersInResultString()
        {
            var sut = _queryExecutor?.ExecuteQuery("no-num", "scenario", 100);
            Assert.That(sut?.NumData[Data.EXECUTION_TIME], Is.EqualTo(0));
            Assert.That(sut?.NumData[Data.PLANNING_TIME], Is.EqualTo(0));
        }

        [Test]
        public void WillTimeout()
        {
            var sut = _queryExecutor?.ExecuteQuery("timeout", "scenario", 0);
            Assert.That(sut.Problem, Is.EqualTo("Timeout at 0ms"));
        }
    }
}