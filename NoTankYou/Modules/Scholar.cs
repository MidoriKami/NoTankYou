using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.Caching;
using KamiLib.Extensions;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Modules;

public class ScholarConfiguration :GenericSettings
{
}

public class Scholar : BaseModule
{
    public override ModuleName Name => ModuleName.Scholar;
    public override string Command => "sch";
    public override List<uint> ClassJobs { get; } = new() { 28 };
    
    private static ScholarConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Scholar;
    public override GenericSettings GenericSettings => Settings;

    private const int DissipationStatusID = 791;
    private readonly HashSet<uint> characterWaitList = new();
    private bool lastDissipationStatus;
    private readonly Action seleneAction;
    
    public Scholar()
    {
        seleneAction = LuminaCache<Action>.Instance.GetRow(17216)!;
    }

    public override WarningState? EvaluateWarning(PlayerCharacter character)
    {
        if (character.Level < 4) return null;

        var hasPet = character.HasPet();
        var hasDissipation = character.HasStatus(DissipationStatusID);

        // If we had dissipation last frame, but not now, wait a half second
        if (lastDissipationStatus && !hasDissipation)
        {
            characterWaitList.Add(character.ObjectId);
            Task.Delay(500).ContinueWith(_ =>
            {
                characterWaitList.Remove(character.ObjectId);
            });
        }

        lastDissipationStatus = hasDissipation;

        if (characterWaitList.Contains(character.ObjectId)) return null;
        if (!hasPet && !hasDissipation)
        {
            return new WarningState
            {
                MessageLong = Strings.Scholar_WarningText,
                MessageShort = Strings.Scholar_WarningTextShort,
                IconID = seleneAction.Icon,
                IconLabel = seleneAction.Name.RawString,
                Priority = Settings.Priority.Value,
            };
        }

        return null;
    }
}
