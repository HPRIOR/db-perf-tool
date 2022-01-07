using System;
using AutoDbPerf.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AutoDbPerf.Implementations
{
    public class Context : IContext
    {
        private readonly IConfiguration _config;

        public Context(IConfiguration config)
        {
            _config = config;
        }

        public string GetEnv(ContextKey contextKey)
        {
            return contextKey switch
            {
                ContextKey.PGPASSWORD => _config["PGPASSWORD"] ?? "",
                ContextKey.PGUSER => _config["PGUSER"] ?? "",
                ContextKey.PGNAME => _config["PGNAME"] ?? "",
                ContextKey.PGHOST => _config["PGHOST"] ?? "",
                ContextKey.PGPORT => _config["PGPORT"] ?? "5432",
                ContextKey.QUERYPATH => _config["QUERYPATH"] ?? "",
                ContextKey.TARGET => _config["TARGET"] ?? "",
                ContextKey.AVGPRECISION => GetAvgPrecision() ?? "5",
                ContextKey.TIMEOUT => _config["TIMEOUT"] ?? "5",
                ContextKey.OUTPUTTYPE => _config["OUTPUTTYPE"] ?? "CSV",
                ContextKey.OUTPUTDIR => _config["OUTPUTDIR"] ?? "",
                ContextKey.GOOGLECREDPATH => _config["GOOGLECREDPATH"] ?? "",
                ContextKey.GOOGLEPROJECTID => _config["GOOGLEPROJECTID"] ?? "",
                ContextKey.ELASTICINDEX => _config["ELASTICINDEX"] ?? "",
                ContextKey.IGNOREFIRST => _config["IGNOREFIRST"] ?? "",
                ContextKey.ORDER => _config["ORDER"] ?? "",
                ContextKey.HOST => _config["HOST"] ?? "localhost",
                ContextKey.ALTCHCLIENT => _config["ALTCHCLIENT"] ?? "false",
                ContextKey.INDEXV => _config["INDEXV"] ?? "data",
                _ => throw new ArgumentOutOfRangeException(nameof(contextKey), contextKey, null)
            };
        }
        
        private string GetAvgPrecision() 
        {
            if (_config["IGNOREFIRST"] == "true")
            {
                int result;
                bool success  = int.TryParse(_config["AVGPRECISION"], out result);
                if (success)
                    return (result + 1).ToString();
            }

            return _config["AVGPRECISION"];
        }
    }
}