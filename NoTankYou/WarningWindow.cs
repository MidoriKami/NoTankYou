using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Party;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace NoTankYou
{
    public class WarningWindow : Window, IDisposable
    {
        private readonly ImGuiScene.TextureWrap warningImage;

        private readonly ImGuiWindowFlags defaultWindowFlags =
                    ImGuiWindowFlags.NoScrollbar |
                    ImGuiWindowFlags.NoScrollWithMouse |
                    ImGuiWindowFlags.NoTitleBar |
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoResize;

        private readonly ImGuiWindowFlags clickthroughFlags =
                    ImGuiWindowFlags.NoScrollbar |
                    ImGuiWindowFlags.NoTitleBar |
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoResize |
                    ImGuiWindowFlags.NoBackground |
                    ImGuiWindowFlags.NoInputs;

        private readonly Vector2 WindowSize = new(500, 100);

        public bool Active { get; set; }
        public bool Delayed { get; set; }
        public bool Forced { get; set; }

        private int lastPartyCount = 0;
        private List<PartyMember> tankList = new();
        private bool TankStanceFound = false;
        private bool SlowModeDelay = false;

        public WarningWindow(ImGuiScene.TextureWrap warningImage) :
            base("Tank Stance Warning Window")
        {
            this.warningImage = warningImage;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(WindowSize.X, WindowSize.Y),
                MaximumSize = new(WindowSize.X, WindowSize.Y)
            };

            Active = true;
            Delayed = false;
            Forced = false;

            IsOpen = true;
        }

        public override void PreDraw()
        {
            base.PreDraw();
            ImGuiWindowFlags windowflags = Service.Configuration.ShowMoveWarningBanner ? defaultWindowFlags : clickthroughFlags;

            Flags = windowflags;
        }

        public override void Draw()
        {
            // If force show banner is enabled, show it no matter what
            if (Forced)
            {
                ImGui.Image(warningImage.ImGuiHandle, new Vector2(warningImage.Width, warningImage.Height));
            }

            // If window is being disabled
            else if (!Active)
            {
                return;
            }

            else
            {
                // If we aren't waiting for the loading screen to complete
                if (!Delayed)
                {
                    // If we are in a party and in a duty
                    if(Service.PartyList.Length > 0 && Service.Condition[ConditionFlag.BoundByDuty])
                    {
                        CheckForTankStanceAndDraw();
                    }
                }
            }
        }

        private void CheckForTankStance()
        {
            // Checks if party size has changed, if it has, updates tanklist
            UpdateTankList();

            // If there are any tanks in the party
            if (tankList.Count > 0)
            {
                // Check each tank for a tank stance
                foreach (var tank in tankList)
                {
                    if (PartyOperations.IsTankStanceFound(tank))
                    {
                        TankStanceFound = true;
                        return;
                    }
                }

                TankStanceFound = false;
            }
        }

        // Checks all party members for a tank stance then displays the banner
        private void CheckForTankStanceAndDraw()
        {
            // If we aren't being delayed
            if(!SlowModeDelay)
            {
                CheckForTankStance();
            }

            // If we are in potato mode, start a delay if we are not already delaying
            if (Service.Configuration.PotatoMode)
            {
                // If we are not currently delayed
                if (SlowModeDelay != true)
                {
                    SlowModeDelay = true;
                    Task.Delay(500).ContinueWith(t => { SlowModeDelay = false; });
                }
            }

            if (!TankStanceFound && tankList.Count > 0)
            {
                // Display warning banner
                ImGui.Image(warningImage.ImGuiHandle, new Vector2(warningImage.Width, warningImage.Height));
            }
        }

        public void UpdateTankList()
        {
            int partySize = Service.PartyList.Length;

            if (lastPartyCount != partySize)
            {
                tankList = PartyOperations.GetTanksList();
                lastPartyCount = partySize;
            }
        }

        public void PrintDebugData()
        {
            var chat = Service.Chat;
            
            chat.Print($"[NoTankYou][debug] Number of Tanks: {tankList.Count}");

            foreach (var tank in tankList)
            {
                chat.Print($"[NoTankYou][debug] Player: {tank.Name}");
                chat.Print($"[NoTankYou][debug] Stance?: {PartyOperations.IsTankStanceFound(tank)}");
            }
        }

        public void Dispose()
        {
            warningImage.Dispose();
        }

        public override void OnClose()
        {
            base.OnClose();
            IsOpen = true;
        }
    }
}
