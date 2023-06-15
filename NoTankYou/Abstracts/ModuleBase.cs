using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dalamud.Game.DutyState;
using Dalamud.Logging;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using KamiLib.AutomaticUserInterface;
using KamiLib.ChatCommands;
using KamiLib.Commands;
using KamiLib.Commands.temp;
using KamiLib.GameState;
using KamiLib.Utilities;
using NoTankYou.DataModels;
using NoTankYou.Models;
using NoTankYou.Models.Enums;
using NoTankYou.Models.Interfaces;
using NoTankYou.System;

namespace NoTankYou.Abstracts;

public abstract unsafe class ModuleBase : IDisposable
{
    public abstract ModuleName ModuleName { get; }
    public virtual ModuleConfigBase ModuleConfig { get; protected set; } = new();
    public abstract string DefaultShortWarning { get; protected set; }
    public abstract string DefaultLongWarning { get; protected set; }
    protected abstract void EvaluatePlayer(IPlayerData playerData);
    
    public virtual void Dispose() { }

    public List<WarningState> ActiveWarningStates { get; } = new();
    public bool HasWarnings => ActiveWarningStates.Any();

    protected string ModuleCommand => ModuleName.ToString().ToLower();

    protected ModuleBase()
    {
        CommandController.RegisterDoubleTierCommand(EnableModuleHandler, new DoubleTierCommandHandler("EnableModule", ModuleCommand, "enable"));
        CommandController.RegisterDoubleTierCommand(DisableModuleHandler, new DoubleTierCommandHandler("DisableModule", ModuleCommand, "disable"));
        CommandController.RegisterDoubleTierCommand(ToggleModuleHandler, new DoubleTierCommandHandler("ToggleModule", ModuleCommand, "toggle"));
    }

    public void EvaluateWarnings()
    {
        ActiveWarningStates.Clear();
        
        if (!ModuleConfig.Enabled) return;
        if (ModuleConfig.DutiesOnly && !Condition.IsBoundByDuty()) return;
        if (Condition.IsBoundByDuty() && !Service.DutyState.IsDutyStarted) return;
        if (ModuleConfig.DisableInSanctuary && GameMain.IsInSanctuary()) return;

        var groupManager = GroupManager.Instance();
        
        if (ModuleConfig.SoloMode || groupManager->MemberCount is 0)
        {
            if (Service.ClientState.LocalPlayer is { } player)
            {
                var localPlayer = (Character*) player.Address;
                if (localPlayer is not null)
                {
                    EvaluatePlayer(new CharacterPlayerData(localPlayer));
                }
            }
        }
        else
        {
            var adjustedPartyMemberSpan = new Span<PartyMember>(groupManager->PartyMembers, groupManager->MemberCount);

            foreach (var partyMember in adjustedPartyMemberSpan)
            {
                EvaluatePlayer(new PartyMemberPlayerData(&partyMember));
            }
        }
    }
    
    public void DrawConfig() => DrawableAttribute.DrawAttributes(ModuleConfig, SaveConfig);
    
    public virtual void Load()
    {
        PluginLog.Debug($"[{ModuleName}] Loading Module");
        ModuleConfig = LoadConfig();
    }

    public virtual void Unload()
    {
        PluginLog.Debug($"[{ModuleName}] Unloading Module");
    }

    protected T GetConfig<T>() where T : ModuleConfigBase
    {
        if (ModuleConfig is not T castedConfig) throw new Exception($"Unable to Get Config Object as type {typeof(T)}");
        
        return castedConfig;
    }
    
    protected void AddActiveWarning(uint actionId, IPlayerData playerData)
    {
        ActiveWarningStates.Add(new WarningState
        {
            Priority = ModuleConfig.Priority,
            ActionId = actionId,
            Message = ModuleConfig.UseCustomShortWarning ? ModuleConfig.CustomShortWarningText : DefaultShortWarning,
            MessageLong = ModuleConfig.UseCustomLongWarning ? ModuleConfig.CustomLongWarningText : DefaultLongWarning,
            SourcePlayerName = MemoryHelper.ReadStringNullTerminated((nint) playerData.GetName()),
            SourceObjectId = playerData.GetObjectId(),
        });
    }

    private ModuleConfigBase LoadConfig() => FileController.LoadFile<ModuleConfigBase>($"{ModuleName}.config.json", ModuleConfig);
    
    public void SaveConfig() => FileController.SaveFile($"{ModuleName}.config.json", ModuleConfig.GetType(), ModuleConfig);

    private void EnableModuleHandler(params string[] _)
    {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP) return;
        
        ModuleConfig.Enabled = true;
        Chat.Print("Command", $"{(ModuleConfig.Enabled ? "Enabling" : "Disabling")} {ModuleName.GetLabel()} Module");
        SaveConfig();
    }
    
    private void DisableModuleHandler(params string[] _)
    {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP) return;

        ModuleConfig.Enabled = false;
        Chat.Print("Command", $"{(ModuleConfig.Enabled ? "Enabling" : "Disabling")} {ModuleName.GetLabel()} Module");
        SaveConfig();
    }
    
    private void ToggleModuleHandler(params string[] _)
    {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP) return;
        
        ModuleConfig.Enabled = !ModuleConfig.Enabled;
        Chat.Print("Command", $"{(ModuleConfig.Enabled ? "Enabling" : "Disabling")} {ModuleName.GetLabel()} Module");
        SaveConfig();
    }
}