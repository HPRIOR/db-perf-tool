using System.Collections.Generic;
using System.Linq;
using AutoDbPerf.Implementations.Postgres;
using AutoDbPerf.Records;
using NUnit.Framework;

namespace test_auto_db_perf
{
    [TestFixture]
    public class TestPostgresQueryInterpreter
    {
        private PostgresQueryInterpreter? _postgresQueryInterpreter;

        [SetUp]
        public void Setup()
        {
            _postgresQueryInterpreter = new PostgresQueryInterpreter();
        }

        [Test]
        public void ScanPredicateCapturesPlanning()
        {
            var output = new List<string>()
            {
                "Planning time: 0",
                "Some other output"
            };
            var predicate = _postgresQueryInterpreter.InitialScanPredicate();
            var sut = output.Where(str => predicate(str));
            Assert.That(sut.First(), Is.EqualTo("Planning time: 0"));
            Assert.That(sut.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ScanPredicateCapturesExecution()
        {
            var output = new List<string>()
            {
                "Execution time: 0",
                "Some other output"
            };
            var predicate = _postgresQueryInterpreter.InitialScanPredicate();
            var sut = output.Where(str => predicate(str));
            Assert.That(sut.First(), Is.EqualTo("Execution time: 0"));
            Assert.That(sut.Count(), Is.EqualTo(1));
        }

        [Test]
        public void WillReturnErrorMessageInInterpretedCommand()
        {
            var input = new CommandResult(new[] { "" },
                new[] { "This is an error", "With more than one line as output" });
            var sut = _postgresQueryInterpreter.InterpretCommandResult(input);
            Assert.That(sut.ErrorMessage, Is.EqualTo("This is an error\nWith more than one line as output"));
        }
        
        [Test]
        public void WillReturnErrorMessageInInterpretedCommand_WithStdOut()
        {
            var input = new CommandResult(new[] { "I have some output" },
                new[] { "This is an error", "With more than one line as output" });
            var sut = _postgresQueryInterpreter.InterpretCommandResult(input);
            Assert.That(sut.ErrorMessage, Is.EqualTo("This is an error\nWith more than one line as output"));
        }
        
        [Test]
        public void IfErrorFalse_IfNoStdErr()
        {
            var input = new CommandResult(new[] { "I have some output" },
                new List<string>());
            var sut = _postgresQueryInterpreter.InterpretCommandResult(input);
            Assert.That(sut.IsError, Is.False);
        }
        
        [Test]
        public void WillNotReturnErrorMessage_IfNoStdErr()
        {
            var input = new CommandResult(new[] { "I have some output" },
                new[] {""});
            var sut = _postgresQueryInterpreter.InterpretCommandResult(input);
            Assert.That(sut.ErrorMessage, Is.Empty);
        }

        [Test]
        public void WillReturnCorrectPlanningResult()
        {
            var input = new CommandResult(new[] { "Planning time: 10" },
                new List<string>());
             var sut = _postgresQueryInterpreter.InterpretCommandResult(input);
             Assert.That(sut.PlanningTime, Is.EqualTo(10));           
        }
        
        [Test]
        public void WillReturnCorrectExecutionResult()
        {
            var input = new CommandResult(new[] { "Execution time: 10" },
                new List<string>());
             var sut = _postgresQueryInterpreter.InterpretCommandResult(input);
             Assert.That(sut.ExecutionTime, Is.EqualTo(10));           
        }
        
        [Test]
        public void WillReturnCorrectExecutionAndPlanningResult()
        {
            var input = new CommandResult(new[] { "Execution time: 10", "Planning time: 10" },
                new List<string>());
             var sut = _postgresQueryInterpreter.InterpretCommandResult(input);
             Assert.That(sut, Is.EqualTo(new InterpretedCommand(false, 10, 10)));           
        }
    }
}