using System;

namespace AutoDbPerf.Implementations.Exceptions
{
    public class ElasticIndexException : Exception
    {
        public ElasticIndexException(string message) : base(message)
        {
        }
    }
}