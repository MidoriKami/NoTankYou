using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Components;
using NoTankYou.Data.Components;

namespace NoTankYou.Interfaces
{
    public interface IModule
    {
        List<ClassJob> ClassJobs { get; }
        GenericSettings GenericSettings { get; }
        string WarningText { get; }

        bool ShowWarning(PlayerCharacter character);

        bool ShouldShowWarning(PlayerCharacter character)
        {
            if (!GenericSettings.Enabled) return false;
            if (GenericSettings.SoloMode && character.ObjectId != Service.ClientState.LocalPlayer?.ObjectId) return false;
            if (character.CurrentHp == 0) return false;

            return ShowWarning(character);
        }

        WarningState GetWarningState()
        {
            return new WarningState
            {
                Message = WarningText,
                Priority = GenericSettings.Priority,
            };
        }
    }
}
