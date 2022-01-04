using System;
using System.Collections.Generic;
using System.IO;
using AutoDbPerf.Implementations.Exceptions;
using AutoDbPerf.Interfaces;
using AutoDbPerf.Utils;

namespace AutoDbPerf.Implementations.Elastic
{
    public class ElasticCommandGenerator : ICommandGenerator
    {
        private readonly IContext _ctx;
        private readonly Dictionary<string, string> _indices;

        public ElasticCommandGenerator(IContext ctx)
        {
            _ctx = ctx;
            _indices = GetIndices();
        }
        
        private Dictionary<string, string> GetIndices() => new()
        {
            {
                "30-worst-case-large-cells",
                $"{_ctx.GetEnv(ContextKey.INDEXV)}_ru_smpphab,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_mobile_enhancement,{_ctx.GetEnv(ContextKey.INDEXV)}_cn_smpphab,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_headphones_mob_headsets,{_ctx.GetEnv(ContextKey.INDEXV)}_br_smpphab_open_market,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_pointing_dev,{_ctx.GetEnv(ContextKey.INDEXV)}_jp_headphones_mob_headsets,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_br_smpphab,{_ctx.GetEnv(ContextKey.INDEXV)}_jp_headsets_total,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_kettles,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_vacuum_cleaners,{_ctx.GetEnv(ContextKey.INDEXV)}_br_foodpreparation,{_ctx.GetEnv(ContextKey.INDEXV)}_cn_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_cn_washingmachines,{_ctx.GetEnv(ContextKey.INDEXV)}_gb_headsets_total,{_ctx.GetEnv(ContextKey.INDEXV)}_gb_headphones_mob_headsets,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_foodpreparation,{_ctx.GetEnv(ContextKey.INDEXV)}_cn_cooling,{_ctx.GetEnv(ContextKey.INDEXV)}_gb_smpphab,{_ctx.GetEnv(ContextKey.INDEXV)}_za_smpphab,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_washingmachines,{_ctx.GetEnv(ContextKey.INDEXV)}_br_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_core_wearables,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_keying_dev,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_mobile_computing,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_cooling,{_ctx.GetEnv(ContextKey.INDEXV)}_fr_headsets_total,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_hairstylers,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_minibluetoothspeakers"
            },
            {
                "5-worst-case-large-cells",
                $"{_ctx.GetEnv(ContextKey.INDEXV)}_ru_smpphab,{_ctx.GetEnv(ContextKey.INDEXV)}_cn_smpphab,{_ctx.GetEnv(ContextKey.INDEXV)}_br_smpphab,{_ctx.GetEnv(ContextKey.INDEXV)}_gb_smpphab,{_ctx.GetEnv(ContextKey.INDEXV)}_za_smpphab"
            },
            { "baseline", $"{_ctx.GetEnv(ContextKey.INDEXV)}_de_mediatablets" },
            {
                "ptv-europe",
                $"{_ctx.GetEnv(ContextKey.INDEXV)}_no_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_nl_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_sk_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_gr_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_de_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_ch_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_it_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_fr_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_gb_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_hu_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_pt_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_es_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_ua_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_dk_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_tr_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_se_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_be_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_cz_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_at_ptv_flat"
            },
            {
                "sda-germany",
                $"{_ctx.GetEnv(ContextKey.INDEXV)}_no_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_nl_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_sk_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_gr_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_de_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_ch_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_ru_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_it_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_fr_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_gb_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_hu_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_pt_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_es_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_ua_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_dk_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_tr_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_se_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_be_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_cz_ptv_flat,{_ctx.GetEnv(ContextKey.INDEXV)}_at_ptv_flat"
            },
            { "worst-case-large-cell", $"{_ctx.GetEnv(ContextKey.INDEXV)}_ru_smpphab" },
            {
                "mda-france",
                $"{_ctx.GetEnv(ContextKey.INDEXV)}_fr_airconditioning,{_ctx.GetEnv(ContextKey.INDEXV)}_fr_builtinhobs,{_ctx.GetEnv(ContextKey.INDEXV)}_fr_cooking,{_ctx.GetEnv(ContextKey.INDEXV)}_fr_cooling,{_ctx.GetEnv(ContextKey.INDEXV)}_fr_dishwashers,{_ctx.GetEnv(ContextKey.INDEXV)}_fr_freezers,{_ctx.GetEnv(ContextKey.INDEXV)}_fr_hoods,{_ctx.GetEnv(ContextKey.INDEXV)}_fr_microwave_ovens,{_ctx.GetEnv(ContextKey.INDEXV)}_fr_tumble_dryers,{_ctx.GetEnv(ContextKey.INDEXV)}_fr_washingmachines"
            }
        };

        public string GenerateCommand(string queryPath)
        {
            return
                $"-c \"curl -XPOST --header 'Content-Type: application/json' \\\"http://{_ctx.GetEnv(ContextKey.HOST)}:9200/{GetIndex(queryPath)}/_search?request_cache=false&filter_path=took\\\" -d'{GetQueryFromPath(queryPath)}' --silent --show-error\"";
        }

        private string GetQueryFromPath(string queryPath)
        {
            return File.Exists(queryPath)
                ? File.ReadAllText(queryPath)
                    .Replace("\n", "")
                    .Replace("\"", "\\\"")
                : "";
        }

        private string GetIndex(string queryPath)
        {
            var scenario = queryPath.GetScenarioFromPath();
            var elasticIndexFromEnv = _ctx.GetEnv(ContextKey.ELASTICINDEX);
            const string errorMsg = "No elastic index given - set ELASTICINDEX or INDEXV with the appropriate supercell folder names";
            
            switch (elasticIndexFromEnv)
            {
                case "" when _indices.ContainsKey(scenario):
                {
                    if (_indices[scenario] == "") throw new ElasticIndexException(errorMsg);
                    return _indices[scenario];
                }
                case "":
                    throw new ElasticIndexException(errorMsg);
                default:
                    return _ctx.GetEnv(ContextKey.ELASTICINDEX);
            }
        }
    }
}