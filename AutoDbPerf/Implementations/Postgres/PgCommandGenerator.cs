using AutoDbPerf.Interfaces;

namespace AutoDbPerf.Implementations.Postgres
{
    public class PgCommandGenerator : ICommandGenerator
    {
        private readonly IContext _ctx;

        public PgCommandGenerator(IContext ctx)
        {
            _ctx = ctx;
        }

        public string GenerateCommand(string queryPath) =>
            $"-c \"PGPASSWORD={_ctx.GetEnv(ContextKey.PGPASSWORD)} psql " +
            $"-h {_ctx.GetEnv(ContextKey.PGHOST)} " +
            $"-U {_ctx.GetEnv(ContextKey.PGUSER)} " +
            $"--dbname {_ctx.GetEnv(ContextKey.PGNAME)} " +
            $"--port {_ctx.GetEnv(ContextKey.PGPORT)} " +
            "--quiet " +
            "--no-psqlrc " +
            "--set ON_ERROR_STOP=1 " +
            $"-a --file {queryPath}\"";
    }
}