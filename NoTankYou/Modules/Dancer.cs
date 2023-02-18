using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.Caching;
using KamiLib.Extensions;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Modules;

public class DancerConfiguration : GenericSettings
{
}

public class Dancer : BaseModule
{
    public override ModuleName Name => ModuleName.Dancer;
    public override string Command => "dnc";
    public override List<uint> ClassJobs { get; } = new() { 38 };

    private static DancerConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Dancer;
    public override GenericSettings GenericSettings => Settings;

    private const int ClosedPositionStatusId = 1823;

    private readonly Action closedPositionAction;
    
    public Dancer()
    {
        closedPositionAction = LuminaCache<Action>.Instance.GetRow(16006)!;
    }

    public override WarningState? EvaluateWarning(PlayerCharacter character)
    {
        if (character.Level < 60) return null;
        if (Service.PartyList.Length < 2) return null;

        if (!character.HasStatus(ClosedPositionStatusId))
        {
            return new WarningState
            {
                MessageLong = Strings.Dancer_WarningText,
                MessageShort = Strings.Dancer_WarningTextShort,
                IconID = closedPositionAction.Icon,
                IconLabel = closedPositionAction.Name.RawString,
                Priority = Settings.Priority.Value,
            };
        }

        return null;
    }
}