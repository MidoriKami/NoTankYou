using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Atk;
using KamiLib.AutomaticUserInterface;
using KamiLib.Caching;
using KamiLib.ChatCommands;
using KamiLib.Commands;
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
    public abstract IModuleConfigBase ModuleConfig { get; protected set; }
    public abstract string DefaultWarningText { get; protected set; }
    protected abstract bool ShouldEvaluate(IPlayerData playerData);
    protected abstract void EvaluateWarnings(IPlayerData playerData);
    protected List<ulong> SuppressedObjectIds = new();

    private AtkUnitBase* NameplateAddon => (AtkUnitBase*)Service.GameGui.GetAddonByName("NamePlate");
    
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
        CommandController.RegisterDoubleTierCommand(SuppressModuleHandler, new DoubleTierCommandHandler("SuppressModuleHelp", ModuleCommand, "suppress"));
    }

    public void EvaluateWarnings()
    {
        ActiveWarningStates.Clear();

        if (!Node.IsAddonReady(NameplateAddon)) return;
        if (!NameplateAddon->IsVisible) return;
        if (!ModuleConfig.Enabled) return;
        if (NoTankYouSystem.BlacklistController.IsZoneBlacklisted(Service.ClientState.TerritoryType)) return;
        if (NoTankYouSystem.SystemConfig.WaitUntilDutyStart && Condition.IsBoundByDuty() && !Service.DutyState.IsDutyStarted) return;
        if (ModuleConfig.DutiesOnly && !Condition.IsBoundByDuty()) return;
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
        if (SuppressedObjectIds.Contains(player.GetObjectId())) return;

        EvaluateWarnings(player);
    }

    private static bool ConditionNotAllowed() 
        => Condition.Any(ConditionFlag.Jumping61,
            ConditionFlag.Transformed,
            ConditionFlag.InThisState89);

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
    
    public void ZoneChange(ushort newZoneId) => SuppressedObjectIds.Clear();

    protected Span<PartyMember> PartyMemberSpan => new(GroupManager.Instance()->PartyMembers, GroupManager.Instance()->MemberCount);

    protected T GetConfig<T>() where T : IModuleConfigBase
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
        SourceModule = ModuleName,
    });

    protected void AddActiveWarning(uint iconId, string iconLabel, IPlayerData playerData) => ActiveWarningStates.Add(new WarningState
    {
        Priority = ModuleConfig.Priority,
        IconId = iconId,
        IconLabel = iconLabel,
        Message = ModuleConfig.CustomWarning ? ModuleConfig.CustomWarningText : DefaultWarningText,
        SourcePlayerName = playerData.GetName(),
        SourceObjectId = playerData.GetObjectId(),
        SourceModule = ModuleName,
    });

    private IModuleConfigBase LoadConfig() => FileController.LoadFile<IModuleConfigBase>($"{ModuleName}.config.json", ModuleConfig);
    
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

    private void SuppressModuleHandler(params string[] _)
    {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP) return;

        foreach (var warningPlayer in ActiveWarningStates)
        {
            SuppressedObjectIds.Add(warningPlayer.SourceObjectId);
            Chat.Print(Strings.Command, string.Format(Strings.SuppressingWarnings, ModuleName.GetLabel(), warningPlayer.SourcePlayerName));
        }
    }

    private void PrintConfirmation() => Chat.Print(Strings.Command, ModuleConfig.Enabled ? $"{Strings.Enabling} {ModuleName.GetLabel()}" : $"{Strings.Disabling} {ModuleName.GetLabel()}");
}