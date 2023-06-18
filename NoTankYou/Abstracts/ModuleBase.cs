using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using KamiLib.AutomaticUserInterface;
using KamiLib.Caching;
using KamiLib.ChatCommands;
using KamiLib.Commands;
using KamiLib.Commands.temp;
using KamiLib.Utilities;
using NoTankYou.DataModels;
using NoTankYou.Localization;
using NoTankYou.Models;
using NoTankYou.Models.Enums;
using NoTankYou.Models.Interfaces;
using NoTankYou.System;
using Action = Lumina.Excel.GeneratedSheets.Action;
using Condition = KamiLib.GameState.Condition;

namespace NoTankYou.Abstracts;

public abstract unsafe class ModuleBase : IDisposable
{
    public abstract ModuleName ModuleName { get; }
    public virtual ModuleConfigBase ModuleConfig { get; protected set; } = new();
    public abstract string DefaultWarningText { get; protected set; }
    protected abstract bool ShouldEvaluate(IPlayerData playerData);
    protected abstract void EvaluateWarnings(IPlayerData playerData);

    private readonly DeathTracker deathTracker = new();
    
    public virtual void Dispose() { }

    public List<WarningState> ActiveWarningStates { get; } = new();
    public bool HasWarnings => ActiveWarningStates.Any();

    protected string ModuleCommand => ModuleName.ToString().ToLower();

    protected ModuleBase()
    {
        CommandController.RegisterDoubleTierCommand(EnableModuleHandler, new DoubleTierCommandHandler("EnableModuleHelp", ModuleCommand, "enable"));
        CommandController.RegisterDoubleTierCommand(DisableModuleHandler, new DoubleTierCommandHandler("DisableModuleHelp", ModuleCommand, "disable"));
        CommandController.RegisterDoubleTierCommand(ToggleModuleHandler, new DoubleTierCommandHandler("ToggleModuleHelp", ModuleCommand, "toggle"));
    }

    public void EvaluateWarnings()
    {
        ActiveWarningStates.Clear();
        
        if (!ModuleConfig.Enabled) return;
        if (ModuleConfig.DutiesOnly && !Condition.IsBoundByDuty()) return;
        if (NoTankYouSystem.SystemConfig.WaitUntilDutyStart && Condition.IsBoundByDuty() && !Service.DutyState.IsDutyStarted) return;
        if (ModuleConfig.DisableInSanctuary && GameMain.IsInSanctuary()) return;
        if (Condition.IsCrossWorld()) return;

        var groupManager = GroupManager.Instance();
        
        if (ModuleConfig.SoloMode || groupManager->MemberCount is 0)
        {
            if (Service.ClientState.LocalPlayer is not { } player) return;
            
            var localPlayer = (Character*) player.Address;
            if (localPlayer is null) return;
            
            ProcessPlayer(new CharacterPlayerData(localPlayer));
        }
        else
        {
            foreach (var partyMember in PartyMemberSpan)
            {
                ProcessPlayer(new PartyMemberPlayerData(partyMember));
            }
        }
    }
    
    private void ProcessPlayer(IPlayerData player)
    {
        if (ConditionNotAllowed()) return;
        if (player.GetObjectId() is 0xE0000000 or 0) return;
        if (deathTracker.IsDead(player)) return;
        if (!ShouldEvaluate(player)) return;

        EvaluateWarnings(player);
    }

    private bool ConditionNotAllowed() => Service.Condition[ConditionFlag.Jumping61] || 
                                          Service.Condition[ConditionFlag.BetweenAreas] || 
                                          Service.Condition[ConditionFlag.BetweenAreas51];

    public void DrawConfig() => DrawableAttribute.DrawAttributes(ModuleConfig, SaveConfig);
    
    public void Load()
    {
        PluginLog.Debug($"[{ModuleName}] Loading Module");
        ModuleConfig = LoadConfig();
    }

    public void Unload()
    {
        PluginLog.Debug($"[{ModuleName}] Unloading Module");
    }

    protected Span<PartyMember> PartyMemberSpan => new(GroupManager.Instance()->PartyMembers, GroupManager.Instance()->MemberCount);

    protected T GetConfig<T>() where T : ModuleConfigBase
    {
        if (ModuleConfig is not T castedConfig) throw new Exception($"Unable to Get Config Object as type {typeof(T)}");
        
        return castedConfig;
    }
    
    protected void AddActiveWarning(uint actionId, IPlayerData playerData) => ActiveWarningStates.Add(new WarningState
    {
        Priority = ModuleConfig.Priority,
        IconId = LuminaCache<Action>.Instance.GetRow(actionId)!.Icon,
        IconLabel = LuminaCache<Action>.Instance.GetRow(actionId)!.Name.ToDalamudString().ToString(),
        Message = ModuleConfig.CustomWarning ? ModuleConfig.CustomWarningText : DefaultWarningText,
        SourcePlayerName = playerData.GetName(),
        SourceObjectId = playerData.GetObjectId(),
    });

    protected void AddActiveWarning(uint iconId, string iconLabel, IPlayerData playerData) => ActiveWarningStates.Add(new WarningState
    {
        Priority = ModuleConfig.Priority,
        IconId = iconId,
        IconLabel = iconLabel,
        Message = ModuleConfig.CustomWarning ? ModuleConfig.CustomWarningText : DefaultWarningText,
        SourcePlayerName = playerData.GetName(),
        SourceObjectId = playerData.GetObjectId(),
    });

    private ModuleConfigBase LoadConfig() => FileController.LoadFile<ModuleConfigBase>($"{ModuleName}.config.json", ModuleConfig);
    
    public void SaveConfig() => FileController.SaveFile($"{ModuleName}.config.json", ModuleConfig.GetType(), ModuleConfig);

    private void EnableModuleHandler(params string[] _)
    {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP) return;
        
        ModuleConfig.Enabled = true;
        PrintConfirmation();
        SaveConfig();
    }
    
    private void DisableModuleHandler(params string[] _)
    {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP) return;

        ModuleConfig.Enabled = false;
        PrintConfirmation();
        SaveConfig();
    }
    
    private void ToggleModuleHandler(params string[] _)
    {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP) return;
        
        ModuleConfig.Enabled = !ModuleConfig.Enabled;
        PrintConfirmation();
        SaveConfig();
    }

    private void PrintConfirmation() => Chat.Print(Strings.Command, ModuleConfig.Enabled ? $"{Strings.Enabling} {ModuleName.GetLabel()}" : $"{Strings.Disabling} {ModuleName.GetLabel()}");
}