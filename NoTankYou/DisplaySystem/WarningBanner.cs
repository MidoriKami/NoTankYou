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
using Dalamud.Game.ClientState.Keys;

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

        protected TextureWrap WarningIcon;
        protected TextureWrap WarningText;
        protected TextureWrap StatusIcon;

        protected Dictionary<uint, Stopwatch> DeathDictionary = new();

        public bool Visible { get; set; } = false;
        public bool Paused { get; set; } = false;
        public bool Forced { get; set; } = false;
        public bool Disabled { get; set; } = false;

        protected abstract ref Configuration.ModuleSettings Settings { get; }
        protected abstract void UpdateInParty();
        protected abstract void UpdateSolo();

        private readonly ConditionManager ConditionManager = new();

        protected WarningBanner(string windowName, string imageName) : base(windowName)
        {
            var assemblyLocation = Service.PluginInterface.AssemblyLocation.DirectoryName!;
            var WarningPath = Path.Combine(assemblyLocation, $@"images\icon\Warning_Icon.png");
            var WarningTextPath = Path.Combine(assemblyLocation, $@"images\text\{imageName}_Text.png");
            var StatusIconPath = Path.Combine(assemblyLocation, $@"images\icon\{imageName}_Icon.png");

            WarningIcon = Service.PluginInterface.UiBuilder.LoadImage(WarningPath);
            WarningText = Service.PluginInterface.UiBuilder.LoadImage(WarningTextPath);
            StatusIcon = Service.PluginInterface.UiBuilder.LoadImage(StatusIconPath);

            UpdateImageSize();
        }

        protected void PreUpdate()
        {
            IsOpen = Settings.Enabled;

            if (Settings.Reposition == true && Service.Configuration.WindowSnappingEnabled == true)
            {
                TrySnapping();
            }
        }

        public override void Update()
        {
            PreUpdate();

            if (!IsOpen) return;

            Forced = Settings.Forced || Settings.Reposition;

            // Party Mode Enabled
            if (ConditionManager.IsPartyMode())
            {
                UpdateInParty();
            }

            // Solo Mode, Duties Only
            else if (ConditionManager.IsSoloDutiesOnly())
            {
                UpdateSolo();
            }

            // Solo Mode, Everywhere
            else if (ConditionManager.IsSoloEverywhere())
            {
                UpdateSolo();
            }

            else
            {
                Visible = false;
            }
        }

        public override void PreDraw()
        {
            base.PreDraw();

            Flags = Settings.Reposition ? MoveWindowFlags : IgnoreInputFlags;

            if (Settings.PositionChanged == true)
            {
                ImGui.SetNextWindowPos(Settings.BannerPosition);
                Settings.PositionChanged = false;
            }
        }

        public override void Draw()
        {
            Settings.BannerPosition = ImGui.GetWindowPos();

            if (!IsOpen) return;

            if (Forced || (Visible && !Disabled && !Paused) )
            {
                var combinedScaleFactor = Settings.ScaleFactor + Service.Configuration.GlobalScaleFactor;
                var drawPositionX = 5 * combinedScaleFactor;

                if (Settings.ShowShield)
                {
                    ImGui.SetCursorPos(new Vector2(drawPositionX, 0));
                    ImGui.Image(WarningIcon.ImGuiHandle, new Vector2((WarningIcon.Width - 5) * combinedScaleFactor, (WarningIcon.Height- 4) * combinedScaleFactor));
                    drawPositionX += WarningIcon.Width * combinedScaleFactor;
                }

                if (Settings.ShowText)
                {
                    ImGui.SetCursorPos(new Vector2(drawPositionX, 0));
                    ImGui.Image(WarningText.ImGuiHandle, new Vector2((WarningText.Width - 10) * combinedScaleFactor, (WarningText.Height- 4) * combinedScaleFactor));
                    drawPositionX += WarningText.Width * combinedScaleFactor;
                }


                if (Settings.ShowIcon)
                {
                    ImGui.SetCursorPos(new Vector2(drawPositionX, 0));
                    ImGui.Image(StatusIcon.ImGuiHandle, new Vector2((StatusIcon.Width - 5) * combinedScaleFactor, (StatusIcon.Height - 4) * combinedScaleFactor));
                }
            }
        }

        private void TrySnapping()
        {
            if(!IsFocused) return;

            if(Service.KeyState[VirtualKey.CONTROL] == false) return;

            var snappingRange = Settings.BannerSize.Y / 2 - 1;

            var modulesThatArentUs = DisplayManager.AllModuleSettings
                .Where(module => module != Settings && module.Reposition == true && module.Enabled == true);

            foreach (var module in modulesThatArentUs)
            {
                var leftDistance = Math.Abs( module.BannerPosition.X - (Settings.BannerPosition.X + Settings.BannerSize.X) );
                var rightDistance = Math.Abs(Settings.BannerPosition.X - (module.BannerPosition.X + module.BannerSize.X));
                var topDistance = Math.Abs(module.BannerPosition.Y - (Settings.BannerPosition.Y + Settings.BannerSize.Y));
                var bottomDistance = Math.Abs(Settings.BannerPosition.Y - (module.BannerPosition.Y + module.BannerSize.Y));

                var xDistance = Math.Abs(module.BannerPosition.X - Settings.BannerPosition.X);
                var yDistance = Math.Abs(module.BannerPosition.Y - Settings.BannerPosition.Y);

                if (leftDistance < snappingRange && yDistance < snappingRange) 
                {
                    Settings.BannerPosition.X = module.BannerPosition.X - Settings.BannerSize.X;
                    Settings.BannerPosition.Y = module.BannerPosition.Y;
                    Settings.PositionChanged = true;
                } 
                else if (rightDistance < snappingRange && yDistance < snappingRange)
                {
                    Settings.BannerPosition.X = module.BannerPosition.X + module.BannerSize.X;
                    Settings.BannerPosition.Y = module.BannerPosition.Y;
                    Settings.PositionChanged = true;
                }
                else if (topDistance < snappingRange && xDistance < snappingRange)
                {
                    Settings.BannerPosition.X = module.BannerPosition.X;
                    Settings.BannerPosition.Y = module.BannerPosition.Y - Settings.BannerSize.Y;
                    Settings.PositionChanged = true;
                }
                else if (bottomDistance < snappingRange && xDistance < snappingRange)
                {
                    Settings.BannerPosition.X = module.BannerPosition.X;
                    Settings.BannerPosition.Y = module.BannerPosition.Y + module.BannerSize.Y;
                    Settings.PositionChanged = true;
                }
            }
        }

        protected List<PartyMember> GetFilteredPartyList(Func<PartyMember, bool> predicate)
        {
            List<PartyMember> partyMembers = Service.PartyList.Where(predicate).ToList();

            var deadPlayers = GetDeadPlayers(partyMembers);
            partyMembers.RemoveAll(r => deadPlayers.Contains(r.ObjectId));

            return partyMembers;
        }

        protected List<uint> GetDeadPlayers(IEnumerable<PartyMember> members)
        {
            AddDeadPlayersToDeathDictionary(members);

            UpdateDeathDictionary();

            return DeathDictionary.Select(d => d.Key).ToList();
        }

        private void AddDeadPlayersToDeathDictionary(IEnumerable<PartyMember> players)
        {
            var deadPlayers = players
                .Where(p => p.CurrentHP == 0)
                .Select(r => r.ObjectId);

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
            var playersWithElapsedTimers = DeathDictionary
                .Where(p => p.Value.ElapsedMilliseconds >= Service.Configuration.DeathGracePeriod);

            foreach (var (player, timer) in playersWithElapsedTimers)
            {
                DeathDictionary.Remove(player);
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

        public void UpdateImageSize()
        {
            var combinedScaleFactor = Settings.ScaleFactor + Service.Configuration.GlobalScaleFactor;
            var windowSizeX = 5 * combinedScaleFactor;

            if (Settings.ShowShield)
                windowSizeX += (int)(WarningIcon.Width * combinedScaleFactor);

            if (Settings.ShowText)
                windowSizeX += (int)(WarningText.Width * combinedScaleFactor);

            if (Settings.ShowIcon)
                windowSizeX += (int)(StatusIcon.Width * combinedScaleFactor);

            Settings.BannerSize.X = windowSizeX;
            Settings.BannerSize.Y = 75 * combinedScaleFactor;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(Settings.BannerSize.X, Settings.BannerSize.Y),
                MaximumSize = new(Settings.BannerSize.X, Settings.BannerSize.Y)
            };
        }

        public void Dispose()
        {
            WarningIcon.Dispose();
            StatusIcon.Dispose();
            WarningText.Dispose();
        }
    }
}
