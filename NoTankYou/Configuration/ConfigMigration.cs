using System;
using System.Collections.Generic;
using System.Numerics;
using KamiLib.Configuration;
using Newtonsoft.Json.Linq;
using NoTankYou.Configuration.Components;
using NoTankYou.Modules;
using NoTankYou.UserInterface.Windows;

namespace NoTankYou.Configuration;

internal static class ConfigMigration
{
    private static JObject? _parsedJson;

    public static CharacterConfiguration Convert(string fileText)
    {
        _parsedJson = JObject.Parse(fileText);

        return new CharacterConfiguration
        {
            Version = 5,
            CharacterData = GetCharacterData(),

            BannerOverlay = GetBannerOverlaySettings(),
            PartyOverlay = GetPartyOverlaySettings(),
            Blacklist = GetBlacklistSettings(),
            Tank = GetTankSettings(),
            Food = GetFoodSettings(),
            FreeCompany = GetFreeCompanySettings(),
            BlueMage = GetBlueMageSettings(),
            Dancer = GetDancerSettings(),
            Sage = GetSageSettings(),
            Scholar = GetScholarSettings(),
            Summoner = GetSummonerSettings(),
        };
    }

    private static PartyOverlaySettings GetPartyOverlaySettings()
    {
        return new PartyOverlaySettings
        {
            FlashingEffects = GetSettingValue<bool>("DisplaySettings.PartyOverlay.FlashingEffects"),
            JobIcon = GetSettingValue<bool>("DisplaySettings.PartyOverlay.JobIcon"),
            PlayerName = GetSettingValue<bool>("DisplaySettings.PartyOverlay.PlayerName"),
            WarningText = GetSettingValue<bool>("DisplaySettings.PartyOverlay.WarningText"),
            WarningOutlineColor = GetVector4("DisplaySettings.PartyOverlay.WarningOutlineColor"),
            WarningTextColor = GetVector4("DisplaySettings.PartyOverlay.WarningTextColor")
        };
    }

    private static BlacklistSettings GetBlacklistSettings()
    {
        return new BlacklistSettings
        {
            Enabled = GetSettingValue<bool>("SystemSettings.Blacklist.Enabled"),
            BlacklistedZones = GetArray<uint>("SystemSettings.Blacklist.BlacklistedZones"),
        };
    }

    private static TankConfiguration GetTankSettings()
    {
        return new TankConfiguration
        {
            Enabled = GetSettingValue<bool>("ModuleSettings.Tank.Enabled"),
            SoloMode = GetSettingValue<bool>("ModuleSettings.Tank.SoloMode"),
            Priority = GetSettingValue<int>("ModuleSettings.Tank.Priority"),
            DutiesOnly = GetSettingValue<bool>("ModuleSettings.Tank.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),

            CheckAllianceStances = GetSettingValue<bool>("ModuleSettings.Tank.CheckAllianceStances"),
            DisableInAllianceRaid = GetSettingValue<bool>("ModuleSettings.Tank.DisableInAllianceRaid"),
        };
    }

    private static FoodConfiguration GetFoodSettings()
    {
        return new FoodConfiguration
        {
            Enabled = GetSettingValue<bool>("ModuleSettings.Food.Enabled"),
            SoloMode = GetSettingValue<bool>("ModuleSettings.Food.SoloMode"),
            Priority = GetSettingValue<int>("ModuleSettings.Food.Priority"),
            DutiesOnly = GetSettingValue<bool>("ModuleSettings.Food.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),

            DisableInCombat = GetSettingValue<bool>("ModuleSettings.Food.DisableInCombat"),
            ExtremeUnreal = GetSettingValue<bool>("ModuleSettings.Food.ExtremeUnreal"),
            FoodEarlyWarningTime = GetSettingValue<int>("ModuleSettings.Food.FoodEarlyWarningTime"),
            SavageDuties = GetSettingValue<bool>("ModuleSettings.Food.SavageDuties"),
            UltimateDuties = GetSettingValue<bool>("ModuleSettings.Food.UltimateDuties"),
        };
    }

    private static FreeCompanyConfiguration GetFreeCompanySettings()
    {
        return new FreeCompanyConfiguration
        {
            Enabled = GetSettingValue<bool>("ModuleSettings.FreeCompany.Enabled"),
            SoloMode = GetSettingValue<bool>("ModuleSettings.FreeCompany.SoloMode"),
            Priority = GetSettingValue<int>("ModuleSettings.FreeCompany.Priority"),
            DutiesOnly = GetSettingValue<bool>("ModuleSettings.FreeCompany.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),

            BuffCount = GetSettingValue<int>("ModuleSettings.FreeCompany.BuffCount"),
            ScanMode = GetSettingEnum<FreeCompanyBuffScanMode>("ModuleSettings.FreeCompany.ScanMode"),
            BuffList = GetBuffList("ModuleSettings.FreeCompany.BuffList"),
        };
    }

    private static BlueMageConfiguration GetBlueMageSettings()
    {
        return new BlueMageConfiguration
        {
            Enabled = GetSettingValue<bool>("ModuleSettings.BlueMage.Enabled"),
            SoloMode = GetSettingValue<bool>("ModuleSettings.BlueMage.SoloMode"),
            Priority = GetSettingValue<int>("ModuleSettings.BlueMage.Priority"),
            DutiesOnly = GetSettingValue<bool>("ModuleSettings.BlueMage.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),

            TankStance = GetSettingValue<bool>("ModuleSettings.BlueMage.TankStance"),
            BasicInstinct = GetSettingValue<bool>("ModuleSettings.BlueMage.BasicInstinct"),
            Mimicry = GetSettingValue<bool>("ModuleSettings.BlueMage.Mimicry"),
        };
    }

    private static DancerConfiguration GetDancerSettings()
    {
        return new DancerConfiguration
        {
            Enabled = GetSettingValue<bool>("ModuleSettings.Dancer.Enabled"),
            SoloMode = GetSettingValue<bool>("ModuleSettings.Dancer.SoloMode"),
            Priority = GetSettingValue<int>("ModuleSettings.Dancer.Priority"),
            DutiesOnly = GetSettingValue<bool>("ModuleSettings.Dancer.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),
        };
    }

    private static SageConfiguration GetSageSettings()
    {
        return new SageConfiguration
        {
            Enabled = GetSettingValue<bool>("ModuleSettings.Sage.Enabled"),
            SoloMode = GetSettingValue<bool>("ModuleSettings.Sage.SoloMode"),
            Priority = GetSettingValue<int>("ModuleSettings.Sage.Priority"),
            DutiesOnly = GetSettingValue<bool>("ModuleSettings.Sage.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),
        };
    }

    private static ScholarConfiguration GetScholarSettings()
    {
        return new ScholarConfiguration
        {
            Enabled = GetSettingValue<bool>("ModuleSettings.Scholar.Enabled"),
            SoloMode = GetSettingValue<bool>("ModuleSettings.Scholar.SoloMode"),
            Priority = GetSettingValue<int>("ModuleSettings.Scholar.Priority"),
            DutiesOnly = GetSettingValue<bool>("ModuleSettings.Scholar.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),
        };
    }

    private static SummonerConfiguration GetSummonerSettings()
    {
        return new SummonerConfiguration
        {
            Enabled = GetSettingValue<bool>("ModuleSettings.Summoner.Enabled"),
            SoloMode = GetSettingValue<bool>("ModuleSettings.Summoner.SoloMode"),
            Priority = GetSettingValue<int>("ModuleSettings.Summoner.Priority"),
            DutiesOnly = GetSettingValue<bool>("ModuleSettings.Summoner.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),
        };
    }

    private static BannerOverlaySettings GetBannerOverlaySettings()
    {
        return new BannerOverlaySettings
        {
            WarningText = GetSettingValue<bool>("DisplaySettings.BannerOverlay.WarningText"),
            Icon = GetSettingValue<bool>("DisplaySettings.BannerOverlay.Icon"),
            IconText = GetSettingValue<bool>("DisplaySettings.BannerOverlay.IconText"),
            Mode = GetSettingEnum<BannerOverlayDisplayMode>("DisplaySettings.BannerOverlay.Mode"),
            PlayerNames = GetSettingValue<bool>("DisplaySettings.BannerOverlay.PlayerNames"),
            Scale = GetSettingValue<float>("DisplaySettings.BannerOverlay.Scale"),
            WarningCount = GetSettingValue<int>("DisplaySettings.BannerOverlay.WarningCount"),
            WarningShield = GetSettingValue<bool>("DisplaySettings.BannerOverlay.WarningShield"),
            BorderThickness = new Setting<float>(1.0f)
        };
    }

    private static CharacterData GetCharacterData()
    {
        var playerData = Service.ClientState.LocalPlayer;
        var contentId = Service.ClientState.LocalContentId;

        var playerName = playerData?.Name.TextValue ?? "Unknown";
        var playerWorld = playerData?.HomeWorld.GameData?.Name.ToString() ?? "UnknownWorld";

        return new CharacterData
        {
            Name = playerName,
            LocalContentID = contentId,
            World = playerWorld,
        };
    }

    private static Setting<T> GetSettingValue<T>(string key) where T : struct
    {
        return new Setting<T>(_parsedJson!.SelectToken(key)!.Value<T>());
    }

    private static Setting<T> GetSettingEnum<T>(string key) where T : struct
    {
        var readValue = _parsedJson!.SelectToken(key)!.Value<int>();

        return new Setting<T>((T) Enum.ToObject(typeof(T), readValue));
    }

    private static T GetValue<T>(string key)
    {
        return _parsedJson!.SelectToken(key)!.Value<T>()!;
    }

    private static JArray GetArray(string key)
    {
        return (JArray) _parsedJson!.SelectToken(key)!;
    }

    private static List<T> GetArray<T>(string key)
    {
        var array = GetArray(key);

        return array.ToObject<List<T>>()!;
    }

    private static uint[] GetBuffList(string key)
    {
        var array = GetArray(key);

        return array.ToObject<uint[]>()!;
    }

    private static Setting<Vector4> GetVector4(string key)
    {
        return new Setting<Vector4>(new Vector4
        {
            X = GetValue<float>($"{key}.X"),
            Y = GetValue<float>($"{key}.Y"),
            Z = GetValue<float>($"{key}.Z"),
            W = GetValue<float>($"{key}.W"),
        });
    }
}