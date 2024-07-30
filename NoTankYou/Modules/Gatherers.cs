using System.Collections.Generic;
using ImGuiNET;
using NoTankYou.Classes;
using NoTankYou.Localization;
using NoTankYou.PlayerDataInterface;

namespace NoTankYou.Modules;

public class Gatherers : ModuleBase<GatherersConfig> {
    public override ModuleName ModuleName => ModuleName.Gatherers;
    protected override string DefaultWarningText => Strings.StatusMissing;

    private const uint MinerClassJobId = 16;
    private const uint BotanistClassJobId = 17;
    private const uint FisherClassJobId = 18;
    
    private readonly List<(uint ClassJob, uint MinLevel, uint StatusId, uint ActionId)> data = [
        (MinerClassJobId, 1, 225, 227),
        (MinerClassJobId, 46, 222, 238),
        (BotanistClassJobId, 1, 217, 210),
        (BotanistClassJobId, 46, 221, 221),
        (FisherClassJobId, 61, 1166, 7903),
        (FisherClassJobId, 65, 1173, 7911),
        (FisherClassJobId, 50, 805, 4101),
    ];
    
    protected override bool ShouldEvaluate(IPlayerData playerData) 
        => playerData.GetClassJob() is 16 or 17 or 18;

    protected override void EvaluateWarnings(IPlayerData playerData) {
        foreach (var (classJob, minLevel, statusId, actionId) in data) {
            if (!Config.Miner && classJob is MinerClassJobId) continue;
            if (!Config.Botanist && classJob is BotanistClassJobId) continue;
            if (!Config.Fisher && classJob is FisherClassJobId) continue;

            // Collectors Glove
            if (statusId is 805 && (!Config.CollectorsGlove || !playerData.HasClassJob(FisherClassJobId))) continue;

            if (playerData.GetLevel() >= minLevel && playerData.MissingStatus(statusId)) {
                AddActiveWarning(actionId, playerData);
            }
        }
    }
}

public class GatherersConfig() : ModuleConfigBase(ModuleName.Gatherers) {
    protected override OptionDisableFlags OptionDisableFlags => OptionDisableFlags.SoloMode | OptionDisableFlags.DutiesOnly;

    public bool Miner = true;
    public bool Botanist = true;
    public bool Fisher = true;
    public bool CollectorsGlove = true;

    protected override void DrawModuleConfig() {
        ConfigChanged |= ImGui.Checkbox(Strings.Miner, ref Miner);
        ConfigChanged |= ImGui.Checkbox(Strings.Botanist, ref Botanist);
        ConfigChanged |= ImGui.Checkbox(Strings.Fisher, ref Fisher);

        if (Fisher) {
            ConfigChanged |= ImGui.Checkbox(Strings.CollectorsGlove, ref CollectorsGlove);
        }
    }
}