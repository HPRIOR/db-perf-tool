using AutoDbPerf.Utils;
using NUnit.Framework;

namespace test_auto_db_perf
{
    [TestFixture]
    public class TestStringUtils
    {
        [Test]
        public void GetScenarioFromPath_WillGetScenario()
        {
            var path = "/Users/harry.prior/Code/supercell-scratch/elastic-queries/ptv_europe/brand-leaderboard.json";
            var sut = path.GetScenarioFromPath();
            Assert.That(sut, Is.EqualTo("ptv_europe"));
        }
        
        [Test]
        public void GetQueryNameFromPath_WithGetQueryName()
        {
            var path = "/Users/harry.prior/Code/supercell-scratch/elastic-queries/ptv_europe/brand-leaderboard.json";
            var sut = path.GetQueryNameFromPath();
            Assert.That(sut, Is.EqualTo("brand-leaderboard"));
        }
        
    }
}