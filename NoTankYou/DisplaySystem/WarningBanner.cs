using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Party;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using ImGuiNET;
using ImGuiScene;
using System;
using System.IO;
using System.Numerics;

namespace NoTankYou.DisplaySystem
{
    public abstract class WarningBanner : Window, IDisposable
    {
        public const ImGuiWindowFlags MoveWindowFlags =
                    ImGuiWindowFlags.NoScrollbar |
                    ImGuiWindowFlags.NoScrollWithMouse |
                    ImGuiWindowFlags.NoTitleBar |
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoBringToFrontOnFocus |
                    ImGuiWindowFlags.NoFocusOnAppearing |
                    ImGuiWindowFlags.NoNavFocus |
                    ImGuiWindowFlags.NoResize;


        public const ImGuiWindowFlags IgnoreInputFlags =
                    ImGuiWindowFlags.NoScrollbar |
                    ImGuiWindowFlags.NoTitleBar |
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoResize |
                    ImGuiWindowFlags.NoBackground |
                    ImGuiWindowFlags.NoBringToFrontOnFocus |
                    ImGuiWindowFlags.NoFocusOnAppearing |
                    ImGuiWindowFlags.NoNavFocus |
                    ImGuiWindowFlags.NoInputs;

        protected TextureWrap Image_Large;
        protected TextureWrap Image_Medium;
        protected TextureWrap Image_Small;
        protected TextureWrap SelectedImage;

        public bool Visible { get; set; } = false;
        public bool Paused { get; set; } = false;
        public bool Forced { get; set; } = false;
        public bool Disabled { get; set; } = false;

        protected abstract ref bool RepositionModeBool { get; }
        protected abstract ref bool ForceShowBool { get; }
        protected abstract ref bool SoloModeBool { get; }

        protected abstract void UpdateInPartyInDuty();
        protected abstract void UpdateSoloInDuty();

        public enum ImageSize
        {
            Small,
            Medium,
            Large
        }

        protected WarningBanner(string windowName, string imageName) : base(windowName)
        {
            var assemblyLocation = Service.PluginInterface.AssemblyLocation.DirectoryName!;
            var smallPath = Path.Combine(assemblyLocation, $@"images\{imageName}_Small.png");
            var mediumPath = Path.Combine(assemblyLocation, $@"images\{imageName}_Medium.png");
            var largePath = Path.Combine(assemblyLocation, $@"images\{imageName}_Large.png");

            Image_Small = Service.PluginInterface.UiBuilder.LoadImage(smallPath);
            Image_Medium = Service.PluginInterface.UiBuilder.LoadImage(mediumPath);
            Image_Large = Service.PluginInterface.UiBuilder.LoadImage(largePath);

            switch (Service.Configuration.ImageSize)
            {
                case ImageSize.Small:
                    SelectedImage = Image_Small;
                    break;

                case ImageSize.Medium:
                    SelectedImage = Image_Medium;
                    break;

                case ImageSize.Large:
                    SelectedImage = Image_Large;
                    break;

                default:
                    SelectedImage = Image_Large;
                    break;
            }

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(this.SelectedImage.Width, this.SelectedImage.Height),
                MaximumSize = new(this.SelectedImage.Width, this.SelectedImage.Height)
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
                Flags = MoveWindowFlags;
            }
            else
            {
                Flags = IgnoreInputFlags;
            }
        }

        public override void Draw()
        {
            if (!IsOpen) return;

            if (Forced)
            {
                ImGui.SetCursorPos(new Vector2(5, 0));
                ImGui.Image(SelectedImage.ImGuiHandle, new Vector2(SelectedImage.Width, SelectedImage.Height));
                return;
            }

            if (Visible && !Disabled && !Paused)
            {
                ImGui.SetCursorPos(new Vector2(5, 0));
                ImGui.Image(SelectedImage.ImGuiHandle, new Vector2(SelectedImage.Width, SelectedImage.Height));
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

        public void ChangeImageSize(ImageSize size)
        {
            switch (size)
            {
                case ImageSize.Small:
                    SelectedImage = Image_Small;
                    break;

                case ImageSize.Medium:
                    SelectedImage = Image_Medium;
                    break;

                case ImageSize.Large:
                    SelectedImage = Image_Large;
                    break;
            }

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(this.SelectedImage.Width, this.SelectedImage.Height),
                MaximumSize = new(this.SelectedImage.Width, this.SelectedImage.Height)
            };
        }

        public void Dispose()
        {
            Image_Small.Dispose();
            Image_Medium.Dispose();
            Image_Large.Dispose();
        }
    }
}
