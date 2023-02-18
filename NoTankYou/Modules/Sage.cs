using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.Caching;
using KamiLib.Extensions;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Modules;

public class SageConfiguration : GenericSettings
{
}

public class Sage : BaseModule
{
    public override ModuleName Name => ModuleName.Sage;
    public override string Command => "sge";
    public override List<uint> ClassJobs { get; } = new() { 40 };
    
    private static SageConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Sage;
    public override GenericSettings GenericSettings => Settings;

    private const int KardiaStatusID = 2604;
    private readonly Action kardiaAction;
    
    public Sage()
    {
        kardiaAction = LuminaCache<Action>.Instance.GetRow(24285)!;
    }

    public override WarningState? EvaluateWarning(PlayerCharacter character)
    {
        if (character.Level < 4) return null;

        if (!character.HasStatus(KardiaStatusID))
        {
            return new WarningState
            {
                MessageLong = Strings.Sage_WarningText,
                MessageShort = Strings.Sage_WarningTextShort,
                IconID = kardiaAction.Icon,
                IconLabel = kardiaAction.Name.RawString,
                Priority = Settings.Priority.Value,
            };
        }

        return null;
    }
}