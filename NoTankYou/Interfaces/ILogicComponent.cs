using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.Utilities;
using NoTankYou.DataModels;

namespace NoTankYou.Interfaces;

public interface ILogicComponent
{
    IModule ParentModule { get; }
    List<uint> ClassJobs { get; }

    WarningState? EvaluateWarning(PlayerCharacter character);

    WarningState? ShouldShowWarning(PlayerCharacter character)
    {
        if (!ParentModule.GenericSettings.Enabled.Value) return null;
        if (ParentModule.GenericSettings.DutiesOnly.Value && !DutyState.Instance.IsDutyStarted) return null;
        if (ParentModule.GenericSettings.SoloMode.Value && character.ObjectId != Service.ClientState.LocalPlayer?.ObjectId) return null;
        if (character.IsDead) return null;

        return EvaluateWarning(character);
    }
}