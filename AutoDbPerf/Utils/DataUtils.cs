using System;
using System.Collections.Generic;
using AutoDbPerf.Implementations;

namespace AutoDbPerf.Utils
{
    public static class DataUtils
    {
        private static readonly List<Data> OrderedData = new()
        {
            Data.EXECUTION_TIME,
            Data.AVG_EXECUTION_TIME,
            Data.EXECUTION_STD_DEV,
            Data.PLANNING_TIME,
            Data.AVG_PLANNING_TIME,
            Data.PLANNING_STD_DEV,
            Data.BYTES_BILLED,
            Data.AVG_BYTES_BILLED,
            Data.BYTES_BILLED_STD_DEV,
            Data.BYTES_PROCESSED,
            Data.AVG_BYTES_PROCESSED,
            Data.BYTES_PROCESSED_STD_DEV,
            Data.BI_MODE,
        };
        
        public static string AsString(this Data data) =>
            data switch
            {
                Data.AVG_BYTES_BILLED => "AvgBytesBilled",
                Data.AVG_BYTES_PROCESSED => "AvgBytesProcessed",
                Data.AVG_EXECUTION_TIME => "AvgExecutionTime",
                Data.AVG_PLANNING_TIME => "AvgPlanningTime",
                Data.BI_MODE => "BiEngineMode",
                Data.BYTES_BILLED => "BytesBilled",
                Data.BYTES_BILLED_STD_DEV => "StdDev",
                Data.BYTES_PROCESSED => "BytesProcessed",
                Data.BYTES_PROCESSED_STD_DEV => "StdDev",
                Data.EXECUTION_STD_DEV => "StdDev",
                Data.EXECUTION_TIME => "ExecutionTime",
                Data.PLANNING_STD_DEV => "StdDev",
                Data.PLANNING_TIME => "PlanningTime",
                _ => throw new ArgumentOutOfRangeException(nameof(data), data, null)
            };

        public static List<Data> GetFixedOrderedData() => OrderedData;
    }
}