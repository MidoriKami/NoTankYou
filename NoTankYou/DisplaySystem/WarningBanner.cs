using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Party;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using ImGuiNET;
using ImGuiScene;
using System;
using System.Numerics;

namespace NoTankYou.DisplaySystem
{
    internal abstract class WarningBanner : Window, IDisposable
    {
        public const ImGuiWindowFlags moveWindowFlags =
                    ImGuiWindowFlags.NoScrollbar |
                    ImGuiWindowFlags.NoScrollWithMouse |
                    ImGuiWindowFlags.NoTitleBar |
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoBringToFrontOnFocus |
                    ImGuiWindowFlags.NoFocusOnAppearing |
                    ImGuiWindowFlags.NoNavFocus |
                    ImGuiWindowFlags.NoResize;


        public const ImGuiWindowFlags ignoreInputFlags =
                    ImGuiWindowFlags.NoScrollbar |
                    ImGuiWindowFlags.NoTitleBar |
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoResize |
                    ImGuiWindowFlags.NoBackground |
                    ImGuiWindowFlags.NoBringToFrontOnFocus |
                    ImGuiWindowFlags.NoFocusOnAppearing |
                    ImGuiWindowFlags.NoNavFocus |
                    ImGuiWindowFlags.NoInputs;

        protected TextureWrap image;

        public bool Visible { get; set; } = false;
        public bool Paused { get; set; } = false;
        public bool Forced { get; set; } = false;
        public bool Disabled { get; set; } = false;

        protected abstract ref bool RepositionModeBool { get; }
        protected abstract ref bool ForceShowBool { get; }
        protected abstract ref bool SoloModeBool { get; }

        protected abstract void UpdateInPartyInDuty();
        protected abstract void UpdateSoloInDuty();

        public WarningBanner(string windowName, TextureWrap image) : base(windowName)
        {
            this.image = image;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(this.image.Width, this.image.Height),
                MaximumSize = new(this.image.Width, this.image.Height)
            };
        }

        public void Update()
        {
            if (!IsOpen) return;

            Forced = ForceShowBool || RepositionModeBool;

            // If we are in a party, and in a duty
            if (Service.PartyList.Length > 0 && Service.Condition[ConditionFlag.BoundByDuty])
            {
                UpdateInPartyInDuty();
            }

            // If we are in a duty, and have solo mode enabled
            else if (SoloModeBool && Service.Condition[ConditionFlag.BoundByDuty])
            {
                UpdateSoloInDuty();
            }

            else
            {
                Visible = false;
            }
        }

        public override void PreDraw()
        {
            base.PreDraw();

            if (RepositionModeBool)
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
                ImGui.Image(image.ImGuiHandle, new Vector2(image.Width, image.Height));
                return;
            }

            if (Visible && !Disabled && !Paused)
            {
                ImGui.SetCursorPos(new Vector2(5, 0));
                ImGui.Image(image.ImGuiHandle, new Vector2(image.Width, image.Height));
                return;
            }
        }

        protected static unsafe bool IsTargetable(PartyMember partyMember)
        {
            var playerGameObject = partyMember.GameObject;
            if (playerGameObject == null) return false;

            var playerTargetable = ((GameObject*)playerGameObject.Address)->GetIsTargetable();

            return playerTargetable;
        }

        protected static unsafe bool IsTargetable(Dalamud.Game.ClientState.Objects.Types.GameObject gameObject)
        {
            var playerTargetable = ((GameObject*)gameObject.Address)->GetIsTargetable();

            return playerTargetable;
        }
        public void Dispose()
        {
            image.Dispose();
        }
    }
}
