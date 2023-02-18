using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.Caching;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Extensions;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.Utilities;
using Condition = KamiLib.GameState.Condition;

namespace NoTankYou.Modules;

public class BlueMageConfiguration : GenericSettings
{
    public Setting<bool> Mimicry = new(false);
    public Setting<bool> TankStance = new(false);
}

public class BlueMage : BaseModule
{
    public override ModuleName Name => ModuleName.BlueMage;
    public override string Command => "blu";
    public override List<uint> ClassJobs { get; } = new() { 36 };

    private static BlueMageConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.BlueMage;
    public override GenericSettings GenericSettings => Settings;

    private readonly List<uint> mimicryStatusEffects = new() { 2124, 2125, 2126 };
    private const uint MightyGuardStatusEffect = 1719;
    private const uint AethericMimicryTank = 2124;

    private readonly Action mimicryAction;
    private readonly Action mightyGuardAction;
    
    public BlueMage()
    {
        mimicryAction = LuminaCache<Action>.Instance.GetRow(18322)!;
        mightyGuardAction = LuminaCache<Action>.Instance.GetRow(11417)!;
    }

    public override WarningState? EvaluateWarning(PlayerCharacter character)
    {
        if (Settings.Mimicry && !Condition.IsBoundByDuty() && !character.HasStatus(mimicryStatusEffects))
        {
            return MimicryWarning;
        }

        if (Settings.TankStance)
        {
            if (Service.PartyList.Length == 0 && character.HasStatus(AethericMimicryTank) && !character.HasStatus(MightyGuardStatusEffect))
            {
                return TankWarning;
            }
            else
            {
                var tankMages = Service.PartyList
                    .WithJob(ClassJobs)
                    .Alive()
                    .WithStatus(AethericMimicryTank)
                    .ToList();

                if (tankMages.Any() && !tankMages.WithStatus(MightyGuardStatusEffect).Any())
                { 
                    return TankWarning;
                }
            }
        }
            
        return null;
    }

    private WarningState MimicryWarning => new()
    {
        MessageShort = Strings.BlueMage_MimicryLabel,
        MessageLong = Strings.BlueMage_Mimicry,
        IconID = mimicryAction.Icon,
        IconLabel = mimicryAction.Name.ToString(),
        Priority = Settings.Priority.Value,
    };

    private WarningState TankWarning => new()
    {
        MessageShort = Strings.BlueMage_MightyGuardLabel,
        MessageLong = Strings.BlueMage_MightyGuard,
        IconID = mightyGuardAction.Icon,
        IconLabel = mightyGuardAction.Name.ToString(),
        Priority = Settings.Priority.Value,
    };

    public override void DrawConfiguration()
    {
        InfoBox.Instance
            .AddTitle(Strings.Tabs_Settings)
            .AddConfigCheckbox(Strings.Labels_Enabled, Settings.Enabled)
            .AddInputInt(Strings.Labels_Priority, Settings.Priority, 0, 10)
            .Draw();
            
        InfoBox.Instance
            .AddTitle(Strings.Labels_Warnings)
            .AddConfigCheckbox(Strings.BlueMage_MimicryLabel, Settings.Mimicry)
            .AddConfigCheckbox(Strings.BlueMage_MightyGuardLabel, Settings.TankStance)
            .Draw();

        InfoBox.Instance.DrawOverlaySettings(Settings);
            
        InfoBox.Instance.DrawOptions(Settings);
    }
}