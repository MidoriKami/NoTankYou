using System.Collections.Generic;
using Dalamud.Bindings.ImGui;
using NoTankYou.Classes;
using NoTankYou.PlayerDataInterface;

namespace NoTankYou.Modules;

public class Gatherers : ModuleBase<GatherersConfig> {
    public override ModuleName ModuleName => ModuleName.Gatherers;
    protected override string DefaultWarningText => "Status Missing";

    private const uint MinerClassJobId = 16;
    private const uint BotanistClassJobId = 17;
    private const uint FisherClassJobId = 18;

    private record GathererJobData(uint ClassJob, uint MinLevel, uint StatusId, uint ActionId);
    
    private readonly List<GathererJobData> data = [
        new (MinerClassJobId, 1, 225, 227),
        new (MinerClassJobId, 46, 222, 238),
        new (BotanistClassJobId, 1, 217, 210),
        new (BotanistClassJobId, 46, 221, 221),
        new (FisherClassJobId, 61, 1166, 7903),
        new (FisherClassJobId, 65, 1173, 7911),
        new (FisherClassJobId, 50, 805, 4101),
    ];
    
    protected override bool ShouldEvaluate(IPlayerData playerData) 
        => playerData.GetClassJob() is 16 or 17 or 18;

    protected override void EvaluateWarnings(IPlayerData playerData) {
        foreach (var jobData in data) {
            if (!Config.Miner && jobData.ClassJob is MinerClassJobId) continue;
            if (!Config.Botanist && jobData.ClassJob is BotanistClassJobId) continue;
            if (!Config.Fisher && jobData.ClassJob is FisherClassJobId) continue;

            // Collectors Glove
            if (jobData.StatusId is 805 && (!Config.CollectorsGlove || !playerData.HasClassJob(FisherClassJobId))) continue;

            if (playerData.GetLevel() >= jobData.MinLevel && playerData.MissingStatus(jobData.StatusId)) {
                AddActiveWarning(jobData.ActionId, playerData);
            }
        }
    }
}

public class GatherersConfig() : ModuleConfigBase(ModuleName.Gatherers) {
    public override OptionDisableFlags OptionDisableFlags => OptionDisableFlags.SoloMode | OptionDisableFlags.DutiesOnly;

    public bool Miner = true;
    public bool Botanist = true;
    public bool Fisher = true;
    public bool CollectorsGlove = true;

    protected override void DrawModuleConfig() {
        ConfigChanged |= ImGui.Checkbox("Miner", ref Miner);
        ConfigChanged |= ImGui.Checkbox("Botanist", ref Botanist);
        ConfigChanged |= ImGui.Checkbox("Fisher", ref Fisher);

        if (Fisher) {
            ConfigChanged |= ImGui.Checkbox("Collectors Glove", ref CollectorsGlove);
        }
    }
}