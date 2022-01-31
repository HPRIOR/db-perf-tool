using System.Text.Json.Serialization;

namespace AutoDbPerf.Implementations.ClickHouse
{
    public class ClickhouseResponse
    {
        [JsonPropertyName("statistics")] public ClickHouseStatisticsResponse Statistics { get; set; }

        public class ClickHouseStatisticsResponse
        {
            [JsonPropertyName("elapsed")] public double Elapsed { get; set; }

            public double ElapsedMs => Elapsed * 1000;
        }
    }
}