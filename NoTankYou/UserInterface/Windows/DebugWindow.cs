using System;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using NoTankYou.System;
using NoTankYou.UserInterface.Components.InfoBox;
using NoTankYou.Utilities;

namespace NoTankYou.UserInterface.Windows;

public class DebugWindow : Window, IDisposable
{
    public DebugWindow() : base("NoTankYou Debugging")
    {
        
    }
    
    public void Dispose()
    {
        
    }

    public override void Update()
    {
        
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