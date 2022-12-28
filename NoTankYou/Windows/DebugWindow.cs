using System;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.InfoBoxSystem;
using NoTankYou.Commands;
using NoTankYou.System;
using NoTankYou.Utilities;

namespace NoTankYou.Windows;

public class DebugWindow : Window
{
    public DebugWindow() : base("NoTankYou Debugging")
    {
        KamiLib.KamiLib.CommandManager.AddCommand(new DebugWindowCommand());
    }
    
    public override void Draw()
    {
        InfoBox.Instance
            .AddTitle("Condition Lockout")
            .AddString(Condition.ConditionLockout.Elapsed.ToString())
            .AddString($"Condition State: {Condition.SpecialConditions()}")
            .Draw();
        
        InfoBox.Instance
            .AddTitle("PartyListAddon")
            .AddAction(DrawTargetability)
            .Draw();
    }

    private void DrawTargetability()
    {
        foreach (var player in Service.PartyListAddon)
        {
            ImGui.Text($"{player.PlayerCharacter?.Name ?? "Unknown"}: {player.IsTargetable()}");
            ImGui.SameLine();
            ImGui.Text(PartyListAddonData.TimeSinceLastTargetable[player.AgentData.ObjectID].Elapsed.ToString());
        }
    }
}