using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using NoTankYou.Components;
using NoTankYou.Data.Components;

namespace NoTankYou.Interfaces
{
    public interface IModule : ICommand
    {
        List<uint> ClassJobs { get; }
        GenericSettings GenericSettings { get; }
        string WarningText { get; }
        string ModuleCommand { get; }

        bool EvaluateWarning(PlayerCharacter character);

        bool ShouldShowWarning(PlayerCharacter character)
        {
            if (!GenericSettings.Enabled) return false;
            if (GenericSettings.DutiesOnly && !Service.EventManager.DutyStarted) return false;
            if (GenericSettings.SoloMode && character.ObjectId != Service.ClientState.LocalPlayer?.ObjectId) return false;
            if (!Service.HudManager.IsTargetable(character.ObjectId)) return false;
            if (character.CurrentHp == 0) return false;

            return EvaluateWarning(character);
        }

        WarningState GetWarningState()
        {
            return new WarningState
            {
                Message = WarningText,
                Priority = GenericSettings.Priority,
            };
        }

        void ICommand.Execute(string? primaryCommand, string? secondaryCommand)
        {
            if (primaryCommand == ModuleCommand)
            {
                switch (secondaryCommand)
                {
                    case null:
                        GenericSettings.Enabled = !GenericSettings.Enabled;
                        break;

                    case "on":
                        GenericSettings.Enabled = true;
                        break;

                    case "off":
                        GenericSettings.Enabled = false;
                        break;
                }
            }
        }
    }
}
