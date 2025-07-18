using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.Interop;
using ImGuiNET;
using KamiLib.CommandManager;
using KamiLib.Extensions;
using NoTankYou.Localization;
using NoTankYou.PlayerDataInterface;
using Action = Lumina.Excel.Sheets.Action;

namespace NoTankYou.Classes;

public abstract class ModuleBase {
    public abstract ModuleName ModuleName { get; }
    public abstract void Load();
    public abstract void DrawConfigUi();
    public abstract void EvaluateWarnings();
    public List<WarningState> ActiveWarningStates { get; } = [];
    public bool HasWarnings => ActiveWarningStates.Count != 0;
    
    public abstract bool IsEnabled { get; }
}

public abstract unsafe class ModuleBase<T> : ModuleBase, IDisposable where T : ModuleConfigBase, new() {
    protected T Config { get; private set; } = new();
    
    public override bool IsEnabled => Config.Enabled;
    
    protected abstract string DefaultWarningText { get; }
    
    protected string ExtraWarningText { get; set; } = string.Empty;
    
    private readonly HashSet<ulong> suppressedObjectIds = [];
    
    private readonly Dictionary<ulong, Stopwatch> suppressionTimer = new();
    
    private readonly DeathTracker deathTracker = new();

    public virtual void Dispose() { }
    
    private string ModuleCommand => ModuleName.ToString();

    protected ModuleBase() {
        System.CommandManager.RegisterCommand(new ToggleCommandHandler {
            DisableDelegate = DisableModuleHandler,
            EnableDelegate = EnableModuleHandler,
            ToggleDelegate = ToggleModuleHandler,
            BaseActivationPath = $"/{ModuleCommand}",
        });
        
        System.CommandManager.RegisterCommand(new CommandHandler {
            Delegate = SuppressModuleHandler,
            ActivationPath = $"/{ModuleCommand}/suppress",
        });
    }
    
    protected abstract bool ShouldEvaluate(IPlayerData playerData);
    
    protected abstract void EvaluateWarnings(IPlayerData playerData);

    public override void EvaluateWarnings() {
        ActiveWarningStates.Clear();

        if (!Config.Enabled) return;
        if (Service.ClientState.IsPvPExcludingDen) return;
        if (System.BlacklistController.IsZoneBlacklisted(Service.ClientState.TerritoryType)) return;
        if (System.SystemConfig.WaitUntilDutyStart && Service.Condition.IsBoundByDuty() && !Service.DutyState.IsDutyStarted) return;
        if (Config.DutiesOnly && !Service.Condition.IsBoundByDuty() && !Config.OptionDisableFlags.HasFlag(OptionDisableFlags.DutiesOnly)) return;
        if (Config.DisableInSanctuary && TerritoryInfo.Instance()->InSanctuary && !Config.OptionDisableFlags.HasFlag(OptionDisableFlags.Sanctuary)) return;
        if (Service.Condition.IsCrossWorld()) return;
        if (System.SystemConfig.HideInQuestEvent && Service.Condition.IsInCutsceneOrQuestEvent()) return;

        var groupManager = GroupManager.Instance();
        
        if (Config.SoloMode || groupManager->MainGroup.MemberCount is 0) {
            if (Service.ClientState.LocalPlayer is not { } player) return;
            
            var localPlayer = (Character*) player.Address;
            if (localPlayer is null) return;
            
            ProcessPlayer(new CharacterPlayerData(localPlayer));
        }
        else {
            foreach (var partyMember in PartyMemberSpan.PointerEnumerator()) {
                ProcessPlayer(new PartyMemberPlayerData(partyMember));
            }
        }
    }
    
    private void ProcessPlayer(IPlayerData player) {
        if (player.GetEntityId() is 0xE0000000 or 0) return;
        if (HasDisallowedCondition()) return;
        if (HasDisallowedStatus(player)) return;
        if (deathTracker.IsDead(player)) return;
        if (!ShouldEvaluate(player)) return;
        if (suppressedObjectIds.Contains(player.GetEntityId())) return;

        EvaluateWarnings(player);
        EvaluateAutoSuppression(player);
    }

    private void EvaluateAutoSuppression(IPlayerData player) {
        if (Config.AutoSuppress) {
            if (Service.ClientState.LocalPlayer is { EntityId: var playerEntityId } && playerEntityId == player.GetEntityId()) {
                return; // Do not allow auto suppression for the user.
            }
            
            suppressionTimer.TryAdd(player.GetEntityId(), Stopwatch.StartNew());
            if (suppressionTimer.TryGetValue(player.GetEntityId(), out var timer)) {
                if (HasWarnings) {
                    if (timer.Elapsed.TotalSeconds >= Config.AutoSuppressTime) {
                        suppressedObjectIds.Add(player.GetEntityId());
                        Service.Log.Warning($"[{ModuleName}]: Adding {player.GetName()} to auto-suppression list");
                    }
                }
                else {
                    timer.Restart();
                }
            }
        }
    }

    private bool HasDisallowedStatus(IPlayerData player)
        => player.HasStatus(1534);

    private static bool HasDisallowedCondition()
        => Service.Condition.Any(ConditionFlag.Jumping61,
            ConditionFlag.Transformed,
            ConditionFlag.InThisState89);

    public override void DrawConfigUi() {
        var minDimension = MathF.Min(ImGui.GetContentRegionMax().X, ImGui.GetContentRegionMax().Y);
        var moduleInfo = ModuleName.GetAttribute<ModuleIconAttribute>()!;
        ImGui.Image(Service.TextureProvider.GetFromGameIcon(moduleInfo.ModuleIcon).GetWrapOrEmpty().ImGuiHandle, new Vector2(minDimension), Vector2.Zero, Vector2.One, KnownColor.White.Vector() with { W = 0.20f });
        ImGui.SetCursorPos(Vector2.Zero);
        
        Config.DrawConfigUi();
    }

    public override void Load() {
        Service.Log.Debug($"[{ModuleName}] Loading Module");
        Config = ModuleConfigBase.Load<T>(ModuleName);
    }

    protected static Span<PartyMember> PartyMemberSpan 
        => GroupManager.Instance()->MainGroup.PartyMembers[..GroupManager.Instance()->MainGroup.MemberCount];

    protected void AddActiveWarning(uint actionId, IPlayerData playerData) => ActiveWarningStates.Add(new WarningState {
        Priority = Config.Priority,
        IconId = Service.DataManager.GetExcelSheet<Action>().GetRow(actionId).Icon,
        IconLabel = Service.DataManager.GetExcelSheet<Action>().GetRow(actionId).Name.ToString(),
        Message = (Config.CustomWarning ? Config.CustomWarningText : DefaultWarningText) + ExtraWarningText,
        SourcePlayerName = playerData.GetName(),
        SourceEntityId = playerData.GetEntityId(),
        SourceModule = ModuleName,
    });

    protected void AddActiveWarning(uint iconId, string iconLabel, IPlayerData playerData) => ActiveWarningStates.Add(new WarningState {
        Priority = Config.Priority,
        IconId = iconId,
        IconLabel = iconLabel,
        Message = (Config.CustomWarning ? Config.CustomWarningText : DefaultWarningText) + ExtraWarningText,
        SourcePlayerName = playerData.GetName(),
        SourceEntityId = playerData.GetEntityId(),
        SourceModule = ModuleName,
    });
    
    private void EnableModuleHandler(params string[] args) {
        if (!Service.ClientState.IsLoggedIn) return;
        
        Config.Enabled = true;
        PrintConfirmation();
        Config.Save();
    }
    
    private void DisableModuleHandler(params string[] args) {
        if (!Service.ClientState.IsLoggedIn) return;

        Config.Enabled = false;
        PrintConfirmation();
        Config.Save();
    }
    
    private void ToggleModuleHandler(params string[] args) {
        if (!Service.ClientState.IsLoggedIn) return;
        
        Config.Enabled = !Config.Enabled;
        PrintConfirmation();
        Config.Save();
    }

    private void SuppressModuleHandler(params string[] args) {
        if (!Service.ClientState.IsLoggedIn) return;

        foreach (var warningPlayer in ActiveWarningStates) {
            suppressedObjectIds.Add(warningPlayer.SourceEntityId);
            Service.Chat.Print(Strings.Command, string.Format(Strings.SuppressingWarnings, ModuleName.GetDescription(), warningPlayer.SourcePlayerName));
        }
    }

    private void PrintConfirmation() 
        => Service.Chat.Print(Strings.Command, Config.Enabled ? $"{Strings.Enabling} {ModuleName.GetDescription()}" : $"{Strings.Disabling} {ModuleName.GetDescription()}");
}