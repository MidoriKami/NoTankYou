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

    private readonly List<(uint ClassJob, uint MinLevel, uint StatusId, uint ActionId)> data = new()
    {
        ( 16, 1, 255, 227 ),
        ( 16, 46, 222, 238 ),
        ( 17, 1, 217, 210 ),
        ( 17, 46, 221, 221 ),
        ( 16, 61, 1166, 7903 ),
        ( 16, 65, 1173, 7911 ),
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
            
            if (playerData.GetLevel() >= minLevel && playerData.MissingStatus(statusId))
            {
                AddActiveWarning(actionId, playerData);
            }
        }
    }
}