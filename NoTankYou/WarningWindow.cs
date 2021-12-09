using Dalamud.Game;
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

        private readonly ImGuiWindowFlags ignoreInputFlags =
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

        public bool DisplayBanner { get; set; }

        private int lastPartyCount = 0;
        private List<PartyMember> tankList = new();

        private PartyOperations PartyOperations { get; set; } = new();

        public WarningWindow(ImGuiScene.TextureWrap warningImage) :
            base("Tank Stance Warning Window")
        {
            this.warningImage = warningImage;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(WindowSize.X, WindowSize.Y),
                MaximumSize = new(WindowSize.X, WindowSize.Y)
            };

            Service.ClientState.TerritoryChanged += OnTerritoryChanged;

            Active = true;
            Delayed = false;
            Forced = false;
            DisplayBanner = false;

            IsOpen = true;
        }

        private void OnTerritoryChanged(object? sender, ushort e)
        {
            // Force Update
            UpdateTankList(true);
        }

        public void Update()
        {
            // If we are in a party and in a duty
            if (Service.PartyList.Length > 0 && Service.Condition[ConditionFlag.BoundByDuty])
            {
                // Checks if party size has changed, if it has, updates tanklist
                UpdateTankList();

                if (tankList.Count > 0)
                {
                    // Check each tank for a tank stance
                    foreach (var tank in tankList)
                    {
                        if (PartyOperations.IsTankStanceFound(tank))
                        {
                            DisplayBanner = false;
                            break;
                        }
                        else
                        {
                            DisplayBanner = true;
                        }
                    }
                }
            }

            if(Delayed)
            {
                DisplayBanner = false;
            }
        }

        public void PrintStatus()
        {
            Service.Chat.Print($"[NoTankYou][status][warningbanner] Active: {Active}");
            Service.Chat.Print($"[NoTankYou][status][warningbanner] Forced: {Forced}");
            Service.Chat.Print($"[NoTankYou][status][warningbanner] Delayed: {Delayed}");
            Service.Chat.Print($"[NoTankYou][status][warningbanner] DisplayBanner: {DisplayBanner}");
        }

        public override void PreDraw()
        {
            base.PreDraw();
            ImGuiWindowFlags windowflags = Service.Configuration.ShowMoveWarningBanner ? defaultWindowFlags : ignoreInputFlags;

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
            else if ( !Active )
            {
                return;
            }

            else if ( DisplayBanner )
            {
                ImGui.Image(warningImage.ImGuiHandle, new Vector2(warningImage.Width, warningImage.Height));
            }
        }

        public void UpdateTankList(bool force = false)
        {
            int partySize = Service.PartyList.Length;

            if ( (lastPartyCount != partySize) || force )
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
                chat.Print($"[NoTankYou][debug] Stance: {PartyOperations.IsTankStanceFound(tank)}");
            }
        }

        public void Dispose()
        {
            warningImage.Dispose();

            Service.ClientState.TerritoryChanged -= OnTerritoryChanged;
        }

        public override void OnClose()
        {
            base.OnClose();
            IsOpen = true;
        }
    }
}
