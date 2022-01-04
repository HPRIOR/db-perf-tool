using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoDbPerf.Implementations;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class TestDirectoryScanner
    {
        [Test]
        public void WillProduceCorrectQueryResultStructure()
        {
            var directoryScanner = new DirectoryScanner();
            var result =
                directoryScanner.ScanDirectories(Path.Join(Environment.CurrentDirectory,
                    "Resources/SQL-Test-1")).ToArray();

            var scenario = "scenario1";
            var queryList = new List<string>
            {
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-1/scenario1/query1.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-1/scenario1/query2.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-1/scenario1/query3.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-1/scenario1/query4.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-1/scenario1/query5.sql")
            };

            Assert.That(result[0].Scenario, Is.EqualTo(scenario));
            Assert.That(result[0].Queries, Is.EquivalentTo(queryList));
        }

        [Test]
        public void WillProduceCorrectQueryResultStructure_WithMultipleScenarios()
        {
            var directoryScanner = new DirectoryScanner();
            var result =
                directoryScanner.ScanDirectories(Path.Join(Environment.CurrentDirectory,
                    "/Resources/SQL-Test-2")).ToArray();

            var scenario = "scenario1";
            var queryList = new List<string>
            {
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-2/scenario1/query1.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-2/scenario1/query2.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-2/scenario1/query3.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-2/scenario1/query4.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-2/scenario1/query5.sql")
            };

            var scenario2 = "scenario2";
            var queryList2 = new List<string>
            {
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-2/scenario2/query1.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-2/scenario2/query2.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-2/scenario2/query3.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-2/scenario2/query4.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/SQL-Test-2/scenario2/query5.sql")
            };

            Assert.That(result[0].Scenario, Is.EqualTo(scenario));
            Assert.That(result[0].Queries, Is.EquivalentTo(queryList));

            Assert.That(result[1].Scenario, Is.EqualTo(scenario2));
            Assert.That(result[1].Queries, Is.EquivalentTo(queryList2));
        }

        [Test]
        public void WillNotScanDotFiles()
        {
            var directoryScanner = new DirectoryScanner();
            var result =
                directoryScanner.ScanDirectories(Path.Join(Environment.CurrentDirectory,
                    "/Resources/dotfiles")).ToArray();

            var scenario = "scenario1";
            var queryList = new List<string>
            {
                Path.Join(Environment.CurrentDirectory, "Resources/dotfiles/scenario1/query1.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/dotfiles/scenario1/query2.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/dotfiles/scenario1/query3.sql")
            };

            var scenario2 = "scenario2";
            var queryList2 = new List<string>
            {
                Path.Join(Environment.CurrentDirectory, "Resources/dotfiles/scenario2/query1.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/dotfiles/scenario2/query2.sql"),
                Path.Join(Environment.CurrentDirectory, "Resources/dotfiles/scenario2/query3.sql")
            };

            result.ToList().ForEach(x =>
            {
                Console.WriteLine(x.Scenario);
                x.Queries.ToList().ForEach(Console.WriteLine);
            });

            Assert.That(result[0].Scenario, Is.EqualTo(scenario));
            Assert.That(result[0].Queries, Is.EquivalentTo(queryList));

            Assert.That(result[1].Scenario, Is.EqualTo(scenario2));
            Assert.That(result[1].Queries, Is.EquivalentTo(queryList2));
        }
    }
}