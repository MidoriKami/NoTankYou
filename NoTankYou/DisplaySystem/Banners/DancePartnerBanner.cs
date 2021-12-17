using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using System.Linq;
using System.Numerics;

namespace NoTankYou.DisplaySystem
{
    internal class DancePartnerBanner : Window, IWarningBanner
    {
        private TextureWrap dancePartnerImage;
        public bool Visible { get; set; } = false;
        public bool Paused { get; set; } = false;
        public bool Forced { get; set; } = false;
        public bool Disabled { get; set; } = false;

        public DancePartnerBanner(TextureWrap dancePartnerImage) : base("Partner Up Dance Partner Warning Banner")
        {
            this.dancePartnerImage = dancePartnerImage;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(this.dancePartnerImage.Width, this.dancePartnerImage.Height),
                MaximumSize = new(this.dancePartnerImage.Width, this.dancePartnerImage.Height)
            };
        }

        public void Update()
        {
            if (!IsOpen) return;

            Forced = Service.Configuration.ForceShowDancePartnerBanner || Service.Configuration.RepositionModeDancePartnerBanner;

            if (Service.PartyList.Length > 0 && Service.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty])
            {
                // ClassJob 38 is Dancer, get the list of all dancers in the party
                var partyList = Service.PartyList.Where(r => r.ClassJob.Id is 38 && r.Level >= 60);

                // Closed Position Status is 1823, get the list of these dancers with Closed Position
                var membersWithClosedPosition = partyList.Where(r => r.Statuses.Any(s => s.StatusId == 1823) && r.Level >= 60);

                // If these two lists match, then everyone's doing their job
                if (partyList.Count() == membersWithClosedPosition.Count())
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

            if (Service.Configuration.RepositionModeDancePartnerBanner)
            {
                Flags = IWarningBanner.moveWindowFlags;
            }
            else
            {
                Flags = IWarningBanner.ignoreInputFlags;
            }
        }

        public override void Draw()
        {
            if (!IsOpen) return;

            if (Forced)
            {
                ImGui.SetCursorPos(new Vector2(5, 0));
                ImGui.Image(dancePartnerImage.ImGuiHandle, new Vector2(dancePartnerImage.Width, dancePartnerImage.Height));
                return;
            }

            if (Visible && !Disabled && !Paused)
            {
                ImGui.SetCursorPos(new Vector2(5, 0));
                ImGui.Image(dancePartnerImage.ImGuiHandle, new Vector2(dancePartnerImage.Width, dancePartnerImage.Height));
                return;
            }
        }

        public void Dispose()
        {
            dancePartnerImage.Dispose();
        }
    }
}
