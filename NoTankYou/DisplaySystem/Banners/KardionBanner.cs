using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NoTankYou.DisplaySystem
{
    internal class KardionBanner : Window, IWarningBanner
    {
        private TextureWrap kardionImage;

        private readonly ImGuiWindowFlags moveWindowFlags =
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
        public bool Disabled { get;set; } = false;

        public KardionBanner(TextureWrap kardionImage) : base("Partner Up Kardion Warning Banner")
        {
            this.kardionImage = kardionImage;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(this.kardionImage.Width, this.kardionImage.Height),
                MaximumSize = new(this.kardionImage.Width, this.kardionImage.Height)
            };
        }

        public void Update()
        {
            if (!IsOpen) return;

            Forced = Service.Configuration.ForceShowKardionBanner || Service.Configuration.RepositionModeKardionBanner;

            if (Service.PartyList.Length > 0 && Service.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty])
            {
                // ClassJob 40 is Sage, get the list of all Sages in the party
                var partyList = Service.PartyList.Where(r => r.ClassJob.Id is 40);

                // Kardion Status is 2605, get the list of party members with Kardion
                var membersWithKardion = Service.PartyList.Where(r => r.Statuses.Any(s => s.StatusId == 2605));

                // If these two lists match, then everyone's doing their job
                if (partyList.Count() == membersWithKardion.Count())
                {
                    Visible = false;
                }

                // If not, then we need to show the warning banner
                else
                {
                    Visible = true;
                }
            }
            else
            {
                Visible = false;
            }
        }

        public override void PreDraw()
        {
            base.PreDraw();

            if(Service.Configuration.RepositionModeKardionBanner)
            {
                Flags = moveWindowFlags;
            }
            else
            {
                Flags = ignoreInputFlags;
            }
        }

        public override void Draw()
        {
            if (!IsOpen) return;

            if (Forced)
            {
                ImGui.SetCursorPos(new Vector2(5, 0));
                ImGui.Image(kardionImage.ImGuiHandle, new Vector2(kardionImage.Width, kardionImage.Height));
                return;
            }

            if (Visible && !Disabled && !Paused)
            {
                ImGui.SetCursorPos(new Vector2(5, 0));
                ImGui.Image(kardionImage.ImGuiHandle, new Vector2(kardionImage.Width, kardionImage.Height));
                return;
            }
        }

        public void Dispose()
        {
            kardionImage.Dispose();
        }
    }
}
