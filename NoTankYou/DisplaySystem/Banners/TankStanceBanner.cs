using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Party;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace NoTankYou.DisplaySystem
{
    public class TankStanceBanner : Window, IDisposable
    {
        private readonly ImGuiScene.TextureWrap tankStanceImage;

        private readonly ImGuiWindowFlags defaultWindowFlags =
                    ImGuiWindowFlags.NoScrollbar |
                    ImGuiWindowFlags.NoScrollWithMouse |
                    ImGuiWindowFlags.NoTitleBar |
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoResize;

        private readonly ImGuiWindowFlags ignoreInputFlags =
                    ImGuiWindowFlags.NoScrollbar |
                    ImGuiWindowFlags.NoTitleBar |
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoResize |
                    ImGuiWindowFlags.NoBackground |
                    ImGuiWindowFlags.NoInputs;

        public bool Visible { get; set; } = false;
        public bool Paused { get; set; } = false;
        public bool Forced { get; set; } = false;
        public bool Disabled { get; set; } = false;

        private List<uint> TankStances = new();
        private List<uint> BlueMageTankStance = new();

        public TankStanceBanner(ImGuiScene.TextureWrap warningImage) :
            base("NoTankYou Warning Banner Window")
        {
            this.tankStanceImage = warningImage;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(this.tankStanceImage.Width, this.tankStanceImage.Height),
                MaximumSize = new(this.tankStanceImage.Width, this.tankStanceImage.Height)
            };

            // Non-Blue Mage Tank Stances
            TankStances = Service.DataManager.GetExcelSheet<Action>()
                !.Where(r => r.ClassJob.Value?.Role is 1)
                !.Select(r => r.StatusGainSelf.Value!)
                !.Where(r => r.IsPermanent == true)
                .Select(r => r.RowId)
                .ToList();

            // Blue Mage Tank Stances
            BlueMageTankStance = Service.DataManager.GetExcelSheet<Action>()
                !.Where(r => r.ClassJob.Value?.Role is 3)
                !.Select(r => r.StatusGainSelf.Value!)
                !.Where(r => r.IsPermanent == true)
                .Select(r => r.RowId)
                .ToList();
        }

        public void Update()
        {
            Forced = Service.Configuration.ForceShowTankStanceBanner || Service.Configuration.RepositionModeTankStanceBanner;

            // If we are in a party and in a duty
            if (Service.PartyList.Length > 0 && Service.Condition[ConditionFlag.BoundByDuty])
            {
                // Get all the Tanks
                var tanks = Service.PartyList.Where(r => r.ClassJob.GameData.Role is 1);
                var blueMageTanks = Service.PartyList.Where(r => r.ClassJob.Id is 36 && r.Statuses.Any(s => s.StatusId == 2124));

                // Get the Tanks that have a tank stance on
                var tankStances = tanks.Where(r => r.Statuses.Any(s => TankStances.Contains(s.StatusId)));
                var blueMageTankStances = blueMageTanks.Where(r => r.Statuses.Any(s => BlueMageTankStance.Contains(s.StatusId)));

                var totalNumberOfTanks = tanks.Count() + blueMageTanks.Count();
                var totalNumberOfTankStances = tankStances.Count() + blueMageTankStances.Count();

                bool atLeastOneTankInParty = totalNumberOfTanks > 0;
                bool noTankStancesFound = totalNumberOfTankStances == 0;

                if ( noTankStancesFound && atLeastOneTankInParty )
                {
                    Visible = true;
                }
                else
                {
                    Visible = false;
                }
            }
            else
            {
                Visible = false;
            }

            if ( Service.Configuration.EnableTankStanceBanner )
            {
                IsOpen = true;
            }
            else
            {
                IsOpen = false;
            }
        }

        public override void PreDraw()
        {
            base.PreDraw();

            if( Service.Configuration.RepositionModeTankStanceBanner )
            {
                Flags = defaultWindowFlags;
            }
            else
            {
                Flags = ignoreInputFlags;
            }
        }

        public override void Draw()
        {
            if (!IsOpen) return;

            if ( Forced )
            {
                ImGui.SetCursorPos(new Vector2(5, 0));
                ImGui.Image(tankStanceImage.ImGuiHandle, new Vector2(tankStanceImage.Width, tankStanceImage.Height));
                return;
            }

            if (Visible && !Disabled && !Paused)
            {
                ImGui.SetCursorPos(new Vector2(5, 0));
                ImGui.Image(tankStanceImage.ImGuiHandle, new Vector2(tankStanceImage.Width, tankStanceImage.Height));
                return;
            }
        }

        public void Dispose()
        {
            tankStanceImage.Dispose();
        }
    }
}
