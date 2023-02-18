using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.Caching;
using KamiLib.Extensions;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Modules;

public class SummonerConfiguration : GenericSettings
{
}

public class Summoner : BaseModule
{
    public override ModuleName Name => ModuleName.Summoner;
    public override string Command => "smn";
    public override List<uint> ClassJobs { get; } = new() {27, 26};
    
    private static SummonerConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Summoner;
    public override GenericSettings GenericSettings => Settings;
    
    private readonly Action summonCarbuncle;

    public Summoner()
    {
        summonCarbuncle = LuminaCache<Action>.Instance.GetRow(25798)!;
    }

    public override WarningState? EvaluateWarning(PlayerCharacter character)
    {
        if (character.Level < 2) return null;

        if(!character.HasPet())
        {
            return new WarningState
            {
                MessageLong = Strings.Summoner_WarningText,
                MessageShort = Strings.Summoner_WarningTextShort,
                IconID = summonCarbuncle.Icon,
                IconLabel = summonCarbuncle.Name.RawString,
                Priority = Settings.Priority.Value,
            };
        }

        return null;
    }
}