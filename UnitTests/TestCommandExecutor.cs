using System.Collections.Generic;
using AutoDbPerf.Implementations;
using AutoDbPerf.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace test_auto_db_perf
{
    [TestFixture]
    public class TestCommandExecutor
    {
        [SetUp]
        public void Setup()
        {
            var commandGenerator = Substitute.For<ICommandGenerator>();
            commandGenerator.GenerateCommand("hello").Returns("-c \"echo Hello World\"");
            commandGenerator.GenerateCommand("new-lines").Returns("-c \"echo Hello World'\n'With New Line\"");
            commandGenerator.GenerateCommand("error").Returns(" -c \"ech sad\"");
            commandGenerator.GenerateCommand("full")
                .Returns(" -c \"echo Planning Time: 0'\n'Execution Time: 0'\n'Throw away line\"");

            _commandExecutor = new CommandExecutor(commandGenerator);
        }

        private CommandExecutor _commandExecutor;


        [Test]
        public void WillCaptureStdout()
        {
            var sut = _commandExecutor.ExecuteCommand("hello", _ => true);
            Assert.That(sut.Result.Stdout, Is.EquivalentTo(new List<string> { "Hello World" }));
        }

        [Test]
        public void WillCaptureStdout_WithNewLines()
        {
            var sut = _commandExecutor.ExecuteCommand("new-lines", _ => true);
            Assert.That(sut.Result.Stdout, Is.EquivalentTo(new List<string> { "Hello World", "With New Line" }));
        }

        [Test]
        public void WillCaptureStdErr()
        {
            var sut = _commandExecutor.ExecuteCommand("error", _ => true);
            Assert.That(sut.Result.Stderr, Is.EquivalentTo(new List<string> { "/bin/bash: ech: command not found" }));
        }

        [Test]
        public void WillCaptureCorrectLines_WithPredication()
        {
            var sut = _commandExecutor.ExecuteCommand("full",
                str => str.Contains("Planning") || str.Contains("Execution"));
            Assert.That(sut.Result.Stdout,
                Is.EquivalentTo(new List<string> { "Planning Time: 0", "Execution Time: 0" }));
        }
    }
}