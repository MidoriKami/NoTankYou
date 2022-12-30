using System.IO;
using KamiLib.Configuration;
using NoTankYou.DataModels;
using NoTankYou.Modules;
using NoTankYou.Windows;

namespace NoTankYou.Configuration;

internal static class ConfigMigration
{
    public static CharacterConfiguration Convert(FileInfo filePath)
    {
        Migrate.LoadFile(filePath);
        
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
            FlashingEffects = Migrate.GetSettingValue<bool>("DisplaySettings.PartyOverlay.FlashingEffects"),
            JobIcon = Migrate.GetSettingValue<bool>("DisplaySettings.PartyOverlay.JobIcon"),
            PlayerName = Migrate.GetSettingValue<bool>("DisplaySettings.PartyOverlay.PlayerName"),
            WarningText = Migrate.GetSettingValue<bool>("DisplaySettings.PartyOverlay.WarningText"),
            WarningOutlineColor = Migrate.GetVector4("DisplaySettings.PartyOverlay.WarningOutlineColor"),
            WarningTextColor = Migrate.GetVector4("DisplaySettings.PartyOverlay.WarningTextColor"),
        };
    }

    private static BlacklistSettings GetBlacklistSettings()
    {
        return new BlacklistSettings
        {
            Enabled = Migrate.GetSettingValue<bool>("SystemSettings.Blacklist.Enabled"),
            //BlacklistedZoneSettings = GetArray<uint>("SystemSettings.Blacklist.BlacklistedZones"),
        };
    }

    private static TankConfiguration GetTankSettings()
    {
        return new TankConfiguration
        {
            Enabled = Migrate.GetSettingValue<bool>("ModuleSettings.Tank.Enabled"),
            SoloMode = Migrate.GetSettingValue<bool>("ModuleSettings.Tank.SoloMode"),
            Priority = Migrate.GetSettingValue<int>("ModuleSettings.Tank.Priority"),
            DutiesOnly = Migrate.GetSettingValue<bool>("ModuleSettings.Tank.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),

            CheckAllianceStances = Migrate.GetSettingValue<bool>("ModuleSettings.Tank.CheckAllianceStances"),
            DisableInAllianceRaid = Migrate.GetSettingValue<bool>("ModuleSettings.Tank.DisableInAllianceRaid"),
        };
    }

    private static FoodConfiguration GetFoodSettings()
    {
        return new FoodConfiguration
        {
            Enabled = Migrate.GetSettingValue<bool>("ModuleSettings.Food.Enabled"),
            SoloMode = Migrate.GetSettingValue<bool>("ModuleSettings.Food.SoloMode"),
            Priority = Migrate.GetSettingValue<int>("ModuleSettings.Food.Priority"),
            DutiesOnly = Migrate.GetSettingValue<bool>("ModuleSettings.Food.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),

            DisableInCombat = Migrate.GetSettingValue<bool>("ModuleSettings.Food.DisableInCombat"),
            ExtremeUnreal = Migrate.GetSettingValue<bool>("ModuleSettings.Food.ExtremeUnreal"),
            FoodEarlyWarningTime = Migrate.GetSettingValue<int>("ModuleSettings.Food.FoodEarlyWarningTime"),
            SavageDuties = Migrate.GetSettingValue<bool>("ModuleSettings.Food.SavageDuties"),
            UltimateDuties = Migrate.GetSettingValue<bool>("ModuleSettings.Food.UltimateDuties"),
        };
    }

    private static FreeCompanyConfiguration GetFreeCompanySettings()
    {
        return new FreeCompanyConfiguration
        {
            Enabled = Migrate.GetSettingValue<bool>("ModuleSettings.FreeCompany.Enabled"),
            SoloMode = Migrate.GetSettingValue<bool>("ModuleSettings.FreeCompany.SoloMode"),
            Priority = Migrate.GetSettingValue<int>("ModuleSettings.FreeCompany.Priority"),
            DutiesOnly = Migrate.GetSettingValue<bool>("ModuleSettings.FreeCompany.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),

            BuffCount = Migrate.GetSettingValue<int>("ModuleSettings.FreeCompany.BuffCount"),
            ScanMode = Migrate.GetSettingEnum<FreeCompanyBuffScanMode>("ModuleSettings.FreeCompany.ScanMode"),
            BuffList = GetBuffList("ModuleSettings.FreeCompany.BuffList"),
        };
    }

    private static BlueMageConfiguration GetBlueMageSettings()
    {
        return new BlueMageConfiguration
        {
            Enabled = Migrate.GetSettingValue<bool>("ModuleSettings.BlueMage.Enabled"),
            SoloMode = Migrate.GetSettingValue<bool>("ModuleSettings.BlueMage.SoloMode"),
            Priority = Migrate.GetSettingValue<int>("ModuleSettings.BlueMage.Priority"),
            DutiesOnly = Migrate.GetSettingValue<bool>("ModuleSettings.BlueMage.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),

            TankStance = Migrate.GetSettingValue<bool>("ModuleSettings.BlueMage.TankStance"),
            Mimicry = Migrate.GetSettingValue<bool>("ModuleSettings.BlueMage.Mimicry"),
        };
    }

    private static DancerConfiguration GetDancerSettings()
    {
        return new DancerConfiguration
        {
            Enabled = Migrate.GetSettingValue<bool>("ModuleSettings.Dancer.Enabled"),
            SoloMode = Migrate.GetSettingValue<bool>("ModuleSettings.Dancer.SoloMode"),
            Priority = Migrate.GetSettingValue<int>("ModuleSettings.Dancer.Priority"),
            DutiesOnly = Migrate.GetSettingValue<bool>("ModuleSettings.Dancer.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),
        };
    }

    private static SageConfiguration GetSageSettings()
    {
        return new SageConfiguration
        {
            Enabled = Migrate.GetSettingValue<bool>("ModuleSettings.Sage.Enabled"),
            SoloMode = Migrate.GetSettingValue<bool>("ModuleSettings.Sage.SoloMode"),
            Priority = Migrate.GetSettingValue<int>("ModuleSettings.Sage.Priority"),
            DutiesOnly = Migrate.GetSettingValue<bool>("ModuleSettings.Sage.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),
        };
    }

    private static ScholarConfiguration GetScholarSettings()
    {
        return new ScholarConfiguration
        {
            Enabled = Migrate.GetSettingValue<bool>("ModuleSettings.Scholar.Enabled"),
            SoloMode = Migrate.GetSettingValue<bool>("ModuleSettings.Scholar.SoloMode"),
            Priority = Migrate.GetSettingValue<int>("ModuleSettings.Scholar.Priority"),
            DutiesOnly = Migrate.GetSettingValue<bool>("ModuleSettings.Scholar.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),
        };
    }

    private static SummonerConfiguration GetSummonerSettings()
    {
        return new SummonerConfiguration
        {
            Enabled = Migrate.GetSettingValue<bool>("ModuleSettings.Summoner.Enabled"),
            SoloMode = Migrate.GetSettingValue<bool>("ModuleSettings.Summoner.SoloMode"),
            Priority = Migrate.GetSettingValue<int>("ModuleSettings.Summoner.Priority"),
            DutiesOnly = Migrate.GetSettingValue<bool>("ModuleSettings.Summoner.DutiesOnly"),
            PartyFrameOverlay = new Setting<bool>(true),
            BannerOverlay = new Setting<bool>(true),
        };
    }

    private static BannerOverlaySettings GetBannerOverlaySettings()
    {
        return new BannerOverlaySettings
        {
            WarningText = Migrate.GetSettingValue<bool>("DisplaySettings.BannerOverlay.WarningText"),
            Icon = Migrate.GetSettingValue<bool>("DisplaySettings.BannerOverlay.Icon"),
            IconText = Migrate.GetSettingValue<bool>("DisplaySettings.BannerOverlay.IconText"),
            Mode = Migrate.GetSettingEnum<BannerOverlayDisplayMode>("DisplaySettings.BannerOverlay.Mode"),
            PlayerNames = Migrate.GetSettingValue<bool>("DisplaySettings.BannerOverlay.PlayerNames"),
            Scale = Migrate.GetSettingValue<float>("DisplaySettings.BannerOverlay.Scale"),
            WarningCount = Migrate.GetSettingValue<int>("DisplaySettings.BannerOverlay.WarningCount"),
            WarningShield = Migrate.GetSettingValue<bool>("DisplaySettings.BannerOverlay.WarningShield"),
            BorderThickness = new Setting<float>(1.0f),
        };
    }

    private static CharacterData GetCharacterData()
    {
        var playerData = Service.ClientState.LocalPlayer;
        var contentId = Service.ClientState.LocalContentId;

        var playerName = playerData?.Name.TextValue ?? "Unknown";

        return new CharacterData
        {
            Name = playerName,
            LocalContentID = contentId,
        };
    }
    
    private static uint[] GetBuffList(string key)
    {
        var array = Migrate.GetArray(key);

        return array.ToObject<uint[]>()!;
    }
}