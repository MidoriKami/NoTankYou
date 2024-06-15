using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib.Configuration;
using NoTankYou.Classes;

namespace NoTankYou.Controllers;

public class PartyListController : IDisposable {
    private PartyListConfig config = new();
    
    // private readonly PartyMemberOverlay?[] partyMembers = new PartyMemberOverlay[8];

    private static WarningState SampleWarning => new() {
        Message = "NoTankYou Sample Warning",
        Priority = 100,
        IconId = 786,
        IconLabel = "Sample Action",
        SourceEntityId = Service.ClientState.LocalPlayer?.EntityId ?? 0xE000000,
        SourcePlayerName = "Sample Player",
        SourceModule = ModuleName.Test,
    };

    public void Update() {
        // foreach (var member in partyMembers) {
        //     member?.Update();
        // }
    }

    public void Draw(List<WarningState> warnings) {
        // if (!config.Enabled) return;
        //
        // if (config.SampleMode) {
        //     partyMembers[0]?.DrawWarning(SampleWarning);
        //     return;
        // }
        //
        // if (config.SoloMode) {
        //     var warning = warnings
        //         .Where(warning => !config.BlacklistedModules.Contains(warning.SourceModule))
        //         .Where(warning => warning.SourceEntityId == Service.ClientState.LocalPlayer?.EntityId)
        //         .MaxBy(warning => warning.Priority);
        //     
        //     partyMembers[0]?.DrawWarning(warning);
        // }
        // else {
        //     foreach (var partyMember in partyMembers) {
        //         var warning = warnings
        //             .Where(warning => !config.BlacklistedModules.Contains(warning.SourceModule))
        //             .Where(warning => warning.SourceEntityId == partyMember?.ObjectId)
        //             .MaxBy(warning => warning.Priority);
        //     
        //         partyMember?.DrawWarning(warning);
        //     }
        // }
    }

    public void Load() {
        config = PartyListConfig.Load();

        foreach (var index in Enumerable.Range(0, 8)) {
            // partyMembers[index] = new PartyMemberOverlay(config, index);
        }
    }

    public void Unload() {
        // foreach (var member in partyMembers) {
        //     member?.Reset(true);
        // }
    }

    public void Dispose() 
        => Unload();

    public void DrawConfig()
        => config.DrawConfigUi();
}

public class PartyListConfig {
    public bool Enabled { get; set; } = true;
    public bool SoloMode { get; set; } = false;
    public bool SampleMode { get; set; } = false;
    
    public bool WarningText { get; set; } = true;
    public bool PlayerName { get; set; } = true;
    public bool JobIcon { get; set; } = true;
    public bool Animation { get; set; } = true;
    public float AnimationPeriod { get; set; } = 1000;
    
    public Vector4 TextColor { get; set; } = KnownColor.Red.Vector();
    public Vector4 OutlineColor { get; set; } = KnownColor.Red.Vector();
    
    public HashSet<ModuleName> BlacklistedModules { get; set; } = [];

    public void DrawConfigUi() {
        ImGui.Text("not implemented yet");
    }
    
    public static PartyListConfig Load() 
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "PartyListOverlay.config.json", () => new PartyListConfig());

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "PartyListOverlay.config.json", this);
}