﻿using Dalamud.Configuration;
using Dalamud.Plugin;
using NoTankYou.DisplaySystem;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace NoTankYou
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        [Serializable]
        public class ModuleSettings
        {
            public bool Enabled = false;
            public bool Forced = false;
            public bool Reposition = false;
            public Vector2 BannerPosition = new Vector2(300, 300);
            public Vector2 BannerSize = new Vector2(0, 0);
            public bool PositionChanged = false;
            public float ScaleFactor = 1.0f;

            public bool ShowShield = true;
            public bool ShowText = true;
            public bool ShowIcon = true;
        }

        public enum MainMode
        {
            Party,
            Solo
        }
        public enum SubMode
        {
            OnlyInDuty,
            Everywhere
        }

        public int Version { get; set; } = 3;
        public int NumberOfWaitFrames = 30;

        public MainMode ProcessingMainMode = MainMode.Party;
        public SubMode ProcessingSubMode = SubMode.OnlyInDuty;

        public bool DisableInAllianceRaid = true;
        public int TerritoryChangeDelayTime = 8000;
        public int DeathGracePeriod = 5000;
        public float FoodEarlyWarningTime = 600.0f;
        public bool FoodSoloMode = true;

        public bool WindowSnappingEnabled = false;
        public float GlobalScaleFactor = 1.0f;

        public ModuleSettings TankStanceSettings = new ModuleSettings();
        public ModuleSettings DancePartnerSettings = new ModuleSettings();
        public ModuleSettings FaerieSettings = new ModuleSettings();
        public ModuleSettings KardionSettings = new ModuleSettings();
        public ModuleSettings SummonerSettings = new ModuleSettings();
        public ModuleSettings BlueMageSettings = new ModuleSettings();
        public ModuleSettings FoodSettings = new ModuleSettings();

        public List<int> TerritoryBlacklist = new();
        public List<int> FoodTerritoryWhitelist = new();

        public bool ForceWindowUpdate = false;

        [NonSerialized] private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            TerritoryBlacklist ??= new();
            FoodTerritoryWhitelist ??= new();

            TankStanceSettings ??= new ModuleSettings();
            DancePartnerSettings ??= new ModuleSettings();
            FaerieSettings ??= new ModuleSettings();
            KardionSettings ??= new ModuleSettings();
            SummonerSettings ??= new ModuleSettings();
            FoodSettings ??= new ModuleSettings();
            Save();
        }

        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }
    }
}