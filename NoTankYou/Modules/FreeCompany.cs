﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Configuration.Components;
using NoTankYou.Configuration.Enums;
using NoTankYou.Configuration.ModuleSettings;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Components;
using NoTankYou.UserInterface.Components.InfoBox;
using NoTankYou.Utilities;

namespace NoTankYou.Modules;

internal class FreeCompany : IModule
{
    public ModuleName Name => ModuleName.FreeCompany;

    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }

    internal static FreeCompanyConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.FreeCompany;
    public GenericSettings GenericSettings => Settings;

    public FreeCompany()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
    }

    internal class ModuleConfigurationComponent : IConfigurationComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);

        private readonly InfoBox GenericSettings = new();
        private readonly InfoBox OverlaySettings = new();
        private readonly InfoBox ExtraOptions = new();
        private readonly InfoBox BuffCount = new();
        private readonly InfoBox BuffSelectionBox = new();

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            GenericSettings
                .AddTitle(Strings.Common.Tabs.Settings)
                .AddConfigCheckbox(Strings.Common.Labels.Enabled, Settings.Enabled)
                .AddInputInt(Strings.Common.Labels.Priority, Settings.Priority, 0, 10)
                .Draw();

            ExtraOptions
                .AddTitle(Strings.Common.Labels.ModeSelect)
                .AddConfigCombo(Enum.GetValues<FreeCompanyBuffScanMode>(), Settings.ScanMode, FreeCompanyBuffScanModeExtensions.GetLabel)
                .Draw();

            BuffCount
                .AddTitle(Strings.Modules.FreeCompany.BuffCount)
                .AddAction(DrawBuffCount)
                .Draw();

            if (Settings.ScanMode.Value == FreeCompanyBuffScanMode.Specific)
            {
                BuffSelectionBox
                    .AddTitle(Strings.Modules.FreeCompany.BuffSelection)
                    .AddAction(BuffSelection)
                    .Draw();
            }

            OverlaySettings
                .AddTitle(Strings.Common.Labels.DisplayOptions)
                .AddConfigCheckbox(Strings.TabItems.PartyOverlay.Label, Settings.PartyFrameOverlay)
                .AddConfigCheckbox(Strings.TabItems.BannerOverlay.Label, Settings.BannerOverlay)
                .Draw();

        }

        private void BuffSelection()
        {
            for (var i = 0; i < Settings.BuffCount.Value; i++)
            {
                var displayValue = Strings.Common.Labels.Unset;
                if (Settings.BuffList[i] != 0)
                {
                    var status = Service.DataManager.GetExcelSheet<Status>()!.GetRow(Settings.BuffList[i]);

                    if (status != null)
                    {
                        displayValue = status.Name.RawString;
                    }
                }
                
                if (ImGui.BeginCombo($"###BuffSelection{i}", displayValue))
                {
                    foreach (var status in Service.DataManager.GetExcelSheet<Status>()!.Where(status => status.IsFcBuff).OrderBy(s => s.Name.RawString))
                    {
                        if (ImGui.Selectable(status.Name.RawString, status.RowId == Settings.BuffList[i]))
                        {
                            Settings.BuffList[i] = status.RowId;
                            Service.ConfigurationManager.Save();
                        }
                    }

                    ImGui.EndCombo();
                }
            }
        }

        private void DrawBuffCount()
        {
            ImGui.PushItemWidth(200.0f);
            if (ImGui.BeginCombo("###BuffCountCombo", Settings.BuffCount.ToString()))
            {
                if (ImGui.Selectable("1", Settings.BuffCount.Value == 1))
                {
                    Settings.BuffCount.Value = 1;
                    Service.ConfigurationManager.Save();
                }

                if (ImGui.Selectable("2", Settings.BuffCount.Value == 2))
                {
                    Settings.BuffCount.Value = 2;
                    Service.ConfigurationManager.Save();
                }

                ImGui.EndCombo();
            }
        }
    }

    internal class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public List<uint> ClassJobs { get; }

        private readonly List<uint> FreeCompanyStatusIDList;

        private readonly CompanyAction FreeCompanyStatus;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            Settings.SoloMode.Value = true;
            Settings.DutiesOnly.Value = false;

            ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!
                .Select(r => r.RowId)
                .ToList();

            FreeCompanyStatusIDList = Service.DataManager.GetExcelSheet<Status>()!
                .Where(status => status.IsFcBuff)
                .Select(status => status.RowId)
                .ToList();

            FreeCompanyStatus = Service.DataManager.GetExcelSheet<CompanyAction>()!.GetRow(43)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (Service.DutyEventManager.DutyStarted) return null;
            if (!CurrentlyInHomeworld(character)) return null;

            switch (Settings.ScanMode.Value)
            {
                case FreeCompanyBuffScanMode.Any:
                    var fcBuffCount = character.StatusCount(FreeCompanyStatusIDList);
                    if (fcBuffCount < Settings.BuffCount.Value)
                    {
                        return new WarningState
                        {
                            MessageLong = Strings.Modules.FreeCompany.WarningText,
                            MessageShort = Strings.Modules.FreeCompany.WarningTextShort,
                            IconID = (uint)FreeCompanyStatus.Icon,
                            IconLabel = "",
                            Priority = Settings.Priority.Value,
                        };
                    }
                    break;

                case FreeCompanyBuffScanMode.Specific:
                    for (var i = 0; i < Settings.BuffCount.Value; ++i)
                    {
                        var targetStatus = Service.DataManager.GetExcelSheet<Status>()!.GetRow(Settings.BuffList[i])!;

                        if (!character.HasStatus(targetStatus.RowId))
                        {
                            return new WarningState
                            {
                                MessageLong = Strings.Modules.FreeCompany.WarningText,
                                MessageShort = Strings.Modules.FreeCompany.WarningTextShort,
                                IconID = (uint)FreeCompanyStatus.Icon,
                                IconLabel = targetStatus.Name.RawString,
                                Priority = Settings.Priority.Value,
                            };
                        }
                    }
                    break;
            }

            return null;
        }

        private bool CurrentlyInHomeworld(PlayerCharacter character)
        {
            var homeworld = character.HomeWorld.Id;
            var currentWorld = character.CurrentWorld.Id;

            return homeworld == currentWorld;
        }
    }
}