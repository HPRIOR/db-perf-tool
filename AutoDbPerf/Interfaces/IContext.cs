using AutoDbPerf.Implementations;

namespace AutoDbPerf.Interfaces
{
    public interface IContext
    {
        string GetEnv(ContextKey contextKey);
    }
}