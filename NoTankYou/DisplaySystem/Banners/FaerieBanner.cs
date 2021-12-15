using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using System;
using System.Linq;
using System.Numerics;

namespace NoTankYou.DisplaySystem
{
    internal class FaerieBanner : Window, IWarningBanner
    {
        private TextureWrap faerieImage;

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
        public bool Disabled { get; set; } = false;

        public FaerieBanner(TextureWrap faerieImage) : base("Partner Up Faerie Warning Banner")
        {
            this.faerieImage = faerieImage;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(this.faerieImage.Width, this.faerieImage.Height),
                MaximumSize = new(this.faerieImage.Width, this.faerieImage.Height)
            };
        }

        public void Update()
        {
            if (!IsOpen) return;

            Forced = Service.Configuration.ForceShowFaerieBanner || Service.Configuration.RepositionModeFaerieBanner;

            if (Service.PartyList.Length > 0 && Service.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty])
            {
                // Scholar Job id is 28
                var scholarPlayers = Service.PartyList.Where(p => p.ClassJob.Id is 28);

                var scholarPlayerIDs = scholarPlayers.Select(r => r.ObjectId);

                // Get the objects that have owner ids matching those of our scholars
                var objectTable = Service.ObjectTable
                    .Where(r => scholarPlayerIDs.Contains(r.OwnerId));

                // id 791 is dissipation id
                var dissipationEffects = scholarPlayers
                    .Where(r => r.Statuses.Any(s => s.StatusId is 791));


                // If these two lists match, then everyone's doing their job
                if (scholarPlayers.Count() == objectTable.Count() + dissipationEffects.Count())
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

            if (Service.Configuration.RepositionModeFaerieBanner)
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
                ImGui.Image(faerieImage.ImGuiHandle, new Vector2(faerieImage.Width, faerieImage.Height));
                return;
            }

            if (Visible && !Disabled && !Paused)
            {
                ImGui.SetCursorPos(new Vector2(5, 0));
                ImGui.Image(faerieImage.ImGuiHandle, new Vector2(faerieImage.Width, faerieImage.Height));
                return;
            }
        }

        public void Dispose()
        {
            faerieImage.Dispose();
        }
    }
}
