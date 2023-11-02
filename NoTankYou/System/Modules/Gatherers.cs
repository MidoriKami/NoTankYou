using System.Collections.Generic;
using NoTankYou.Abstracts;
using NoTankYou.Localization;
using NoTankYou.Models.Enums;
using NoTankYou.Models.Interfaces;
using NoTankYou.Models.ModuleConfiguration;

namespace NoTankYou.System.Modules;

public class Gatherers : ModuleBase
{
    public override ModuleName ModuleName => ModuleName.Gatherers;
    public override IModuleConfigBase ModuleConfig { get; protected set; } = new GatherersConfig();
    protected override string DefaultWarningText => Strings.StatusMissing;

    private const uint MinerClassJobId = 16;
    private const uint BotanistClassJobId = 17;
    private const uint FisherClassJobId = 18;
    
    private readonly List<(uint ClassJob, uint MinLevel, uint StatusId, uint ActionId)> data = new()
    {
        ( MinerClassJobId, 1, 225, 227 ),
        ( MinerClassJobId, 46, 222, 238 ),
        ( BotanistClassJobId, 1, 217, 210 ),
        ( BotanistClassJobId, 46, 221, 221 ),
        ( FisherClassJobId, 61, 1166, 7903 ),
        ( FisherClassJobId, 65, 1173, 7911 ),
        ( FisherClassJobId, 50, 805, 4101 ),
    };
    
    protected override bool ShouldEvaluate(IPlayerData playerData) 
        => playerData.GetClassJob() is 16 or 17 or 18;

    protected override void EvaluateWarnings(IPlayerData playerData)
    {
        var config = GetConfig<GatherersConfig>();
        
        foreach (var (classJob, minLevel, statusId, actionId) in data)
        {
            if (!config.Miner && classJob is 16) continue;
            if (!config.Botanist && classJob is 17) continue;
            if (!config.Fisher && classJob is 18) continue;
            
            // Collectors Glove
            if (statusId is 805 && !config.CollectorsGlove) continue;

            if (playerData.GetLevel() >= minLevel && playerData.MissingStatus(statusId))
            {
                AddActiveWarning(actionId, playerData);
            }
        }
    }
}