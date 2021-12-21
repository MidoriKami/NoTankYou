using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Party;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using ImGuiNET;
using ImGuiScene;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        protected TextureWrap ImageLarge;
        protected TextureWrap ImageMedium;
        protected TextureWrap ImageSmall;
        protected TextureWrap SelectedImage;

        protected Dictionary<PartyMember, Stopwatch> DeathDictionary = new();

        public bool Visible { get; set; } = false;
        public bool Paused { get; set; } = false;
        public bool Forced { get; set; } = false;
        public bool Disabled { get; set; } = false;

        protected abstract ref bool RepositionModeBool { get; }
        protected abstract ref bool ForceShowBool { get; }
        protected abstract ref bool ModuleEnabled { get; }

        protected abstract void UpdateInPartyInDuty();
        protected abstract void UpdateSoloInDuty();
        protected abstract void UpdateSoloEverywhere();

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

            ImageSmall = Service.PluginInterface.UiBuilder.LoadImage(smallPath);
            ImageMedium = Service.PluginInterface.UiBuilder.LoadImage(mediumPath);
            ImageLarge = Service.PluginInterface.UiBuilder.LoadImage(largePath);

            switch (Service.Configuration.ImageSize)
            {
                case ImageSize.Small:
                    SelectedImage = ImageSmall;
                    break;

                case ImageSize.Medium:
                    SelectedImage = ImageMedium;
                    break;

                case ImageSize.Large:
                    SelectedImage = ImageLarge;
                    break;

                default:
                    SelectedImage = ImageLarge;
                    break;
            }

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(this.SelectedImage.Width, this.SelectedImage.Height),
                MaximumSize = new(this.SelectedImage.Width, this.SelectedImage.Height)
            };
        }

        protected void PreUpdate()
        {
            IsOpen = ModuleEnabled;
        }

        public void Update()
        {
            PreUpdate();

            if (!IsOpen) return;

            Forced = ForceShowBool || RepositionModeBool;

            // Party Mode Enabled
            if ( IsPartyMode() )
            {
                UpdateInPartyInDuty();
            }

            // Solo Mode, Duties Only
            else if ( IsSoloDutiesOnly() )
            {
                UpdateSoloInDuty();
            }

            // Solo Mode, Everywhere
            else if ( IsSoloEverywhere() )
            {
                UpdateSoloEverywhere();
            }

            else
            {
                Visible = false;
            }
        }

        public override void PreDraw()
        {
            base.PreDraw();

            Flags = RepositionModeBool ? MoveWindowFlags : IgnoreInputFlags;
        }

        public override void Draw()
        {
            if (!IsOpen) return;

            if (Forced)
            {
                ImGui.SetCursorPos(new Vector2(5, 0));
                ImGui.Image(SelectedImage.ImGuiHandle, new Vector2(SelectedImage.Width - 5, SelectedImage.Height));
                return;
            }

            if (Visible && !Disabled && !Paused)
            {
                ImGui.SetCursorPos(new Vector2(5, 0));
                ImGui.Image(SelectedImage.ImGuiHandle, new Vector2(SelectedImage.Width - 5, SelectedImage.Height));
                return;
            }
        }

        private bool IsBoundByDuty()
        {
            var baseBoundByDuty = Service.Condition[ConditionFlag.BoundByDuty];
            var boundBy56 = Service.Condition[ConditionFlag.BoundByDuty56];
            var boundBy95 = Service.Condition[ConditionFlag.BoundByDuty95];
            var boundBy97 = Service.Condition[ConditionFlag.BoundToDuty97];

            return baseBoundByDuty || boundBy56 || boundBy95 || boundBy97;
        }

        private bool IsInAreaTransition()
        {
            var baseTransition = Service.Condition[ConditionFlag.BetweenAreas];
            var transition51 = Service.Condition[ConditionFlag.BetweenAreas51];

            return baseTransition || transition51;
        }

        private bool IsPartyMode()
        {
            var inParty = Service.PartyList.Length > 0;
            var isBoundByDuty = IsBoundByDuty();
            var isPartyMode = Service.Configuration.ProcessingMainMode == Configuration.MainMode.Party;
            var isInTransition = IsInAreaTransition();

            return inParty && isBoundByDuty && isPartyMode && !isInTransition;
        }

        private bool IsSoloDutiesOnly()
        {
            var isSoloMainMode = Service.Configuration.ProcessingMainMode == Configuration.MainMode.Solo;
            var isDutiesOnlySubMode = Service.Configuration.ProcessingSubMode == Configuration.SubMode.OnlyInDuty;
            var isBoundBuByDuty = IsBoundByDuty();
            var isInAreaTransition = IsInAreaTransition();

            return isSoloMainMode && isDutiesOnlySubMode && isBoundBuByDuty && !isInAreaTransition;
        }

        private bool IsSoloEverywhere()
        {
            var isSoloMainMode = Service.Configuration.ProcessingMainMode == Configuration.MainMode.Solo;
            var isEverywhereSubMode = Service.Configuration.ProcessingSubMode == Configuration.SubMode.Everywhere;
            var isInAreaTransition = IsInAreaTransition();

            return isSoloMainMode && isEverywhereSubMode && !isInAreaTransition;
        }

        protected void FilterDeadPlayers(ref ICollection<PartyMember> members)
        {
            AddDeadPlayersToDeathDictionary(members);

            UpdateDeathDictionary();

            RemoveDeadPlayers(ref members);
        }

        private void AddDeadPlayersToDeathDictionary(IEnumerable<PartyMember> players)
        {
            var deadPlayers = players.Where(p => p.CurrentHP == 0);

            foreach (var deadPlayer in deadPlayers)
            {
                // If they were dead last check, and are still dead
                if (DeathDictionary.ContainsKey(deadPlayer))
                {
                    // Reset the timer
                    DeathDictionary[deadPlayer].Restart();
                }
                // Else this is the first time we are seeing them dead
                else
                {
                    // Add an start the timer
                    DeathDictionary.Add(deadPlayer, new Stopwatch());
                    DeathDictionary[deadPlayer].Start();
                }
            }
        }

        private void UpdateDeathDictionary()
        {
            var playersWithElapsedTimers =
                DeathDictionary.Where(p => p.Value.ElapsedMilliseconds >= Service.Configuration.DeathGracePeriod);
            
            foreach (var (player, timer) in playersWithElapsedTimers)
            {
                DeathDictionary.Remove(player);
            }
        }

        private void RemoveDeadPlayers(ref ICollection<PartyMember> allPartyMembers)
        {
            var deadMembers = allPartyMembers.Where(member => DeathDictionary.ContainsKey(member));

            foreach (var memberToRemove in deadMembers)
            {
                if (allPartyMembers.Contains(memberToRemove))
                {
                    allPartyMembers.Remove(memberToRemove);
                }
            }
        }

        public static unsafe bool IsTargetable(PartyMember partyMember)
        {
            var playerGameObject = partyMember.GameObject;
            if (playerGameObject == null) return false;

            var playerTargetable = ((GameObject*)playerGameObject.Address)->GetIsTargetable();

            return playerTargetable;
        }

        public static unsafe bool IsTargetable(Dalamud.Game.ClientState.Objects.Types.GameObject gameObject)
        {
            var playerTargetable = ((GameObject*)gameObject.Address)->GetIsTargetable();

            return playerTargetable;
        }

        public void ChangeImageSize(ImageSize size)
        {
            switch (size)
            {
                case ImageSize.Small:
                    SelectedImage = ImageSmall;
                    break;

                case ImageSize.Medium:
                    SelectedImage = ImageMedium;
                    break;

                case ImageSize.Large:
                    SelectedImage = ImageLarge;
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
            ImageSmall.Dispose();
            ImageMedium.Dispose();
            ImageLarge.Dispose();
        }
    }
}
