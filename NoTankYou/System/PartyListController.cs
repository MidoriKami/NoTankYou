using System;
using System.Collections.Generic;
using System.Linq;
using KamiLib.AutomaticUserInterface;
using KamiLib.FileIO;
using NoTankYou.DataModels;
using NoTankYou.Models;
using NoTankYou.Models.Enums;
using NoTankYou.Views.Components;

namespace NoTankYou.System;

public class PartyListController : IDisposable {
    private record MemberWarning(PartyMemberOverlay? UiData, WarningState? Warning);

    private PartyListConfig config = new();
    
    private readonly PartyMemberOverlay?[] partyMembers = new PartyMemberOverlay[8];
    private List<MemberWarning> ActiveWarnings = new();

    private static WarningState SampleWarning => new()
    {
        Message = "NoTankYou Sample Warning",
        Priority = 100,
        IconId = 786,
        IconLabel = "Sample Action",
        SourceObjectId = Service.ClientState.LocalPlayer?.ObjectId ?? 0xE000000,
        SourcePlayerName = "Sample Player",
        SourceModule = ModuleName.Test,
    };

    public void Update()
    {
        foreach (var member in partyMembers)
        {
            member?.Update();
        }
    }

    public void UpdateWarnings(List<WarningState> warnings) {
        ActiveWarnings.Clear();
        if (!config.Enabled) return;

        if (config.SampleMode) {
            ActiveWarnings.Add(new MemberWarning(partyMembers[0], SampleWarning));
            return;
        }

        if (config.SoloMode) {
            var warning = warnings
                .Where(warning => !config.BlacklistedModules.Contains(warning.SourceModule))
                .Where(warning => warning.SourceObjectId == Service.ClientState.LocalPlayer?.ObjectId)
                .MaxBy(warning => warning.Priority);
            
            ActiveWarnings.Add(new MemberWarning(partyMembers[0], warning));
        }
        else {
            foreach (var partyMember in partyMembers) {
                var warning = warnings
                    .Where(warning => !config.BlacklistedModules.Contains(warning.SourceModule))
                    .Where(warning => warning.SourceObjectId == partyMember?.ObjectId)
                    .MaxBy(warning => warning.Priority);
            
                ActiveWarnings.Add(new MemberWarning(partyMember, warning));
            }
        }
    }

    public void DrawImGui() {
        if (!config.Enabled) return;

        foreach (var (member, warning) in ActiveWarnings) {
            if (member is null) continue;
            if (warning is null) continue;
            
            member.DrawWarning(warning);
        }
    }

    public void DrawNative() {
        if (!config.Enabled) return;

        foreach (var (member, warning) in ActiveWarnings) {
            if (member is null) continue;
            if (warning is null) continue;
            
            member.DrawNative(warning);
        }
    }

    public void Load()
    {
        config = LoadConfig();

        foreach (var index in Enumerable.Range(0, 8))
        {
            partyMembers[index] = new PartyMemberOverlay(config, index);
        }
    }

    public void Unload()
    {
        foreach (var member in partyMembers)
        {
            member?.Reset(true);
        }
    }

    public void Dispose() => Unload();
    public void DrawConfig() => DrawableAttribute.DrawAttributes(config, SaveConfig);
    private PartyListConfig LoadConfig() => CharacterFileController.LoadFile<PartyListConfig>("PartyListOverlay.config.json", config);
    public void SaveConfig() => CharacterFileController.SaveFile("PartyListOverlay.config.json", config.GetType(), config);
}