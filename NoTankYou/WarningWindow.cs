using System;
using System.Numerics;
using System.Threading.Tasks;
using ImGuiNET;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using Lumina.Excel.GeneratedSheets;
using Dalamud.Game.ClientState.Party;
using System.Linq;
using System.Collections.Generic;

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
        private List<PartyMember> tankList;

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
                    // Check for tank stance, if one isn't found show warning banner
                    CheckForTankStanceAndDraw();
                }
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

        // Checks all party members for a tank stance then displays the banner
        public void CheckForTankStanceAndDraw()
        {
            UpdateTankList();

            // Is the player in a party? and also in a duty?
            if ( Service.PartyList.Length > 0 && Service.Condition[ConditionFlag.BoundByDuty] )
            {
                // If we found any tanks
                if (tankList.Count > 0)
                {
                    bool tankStanceFound = false;

                    // Check each tank for a tank stance
                    foreach(var tank in tankList)
                    {
                        if (PartyOperations.IsTankStanceFound(tank))
                        {
                            tankStanceFound = true;
                            break;
                        }
                    }
                    
                    // If none of the tanks have their stance on
                    if (!tankStanceFound)
                    {
                        // Display warning banner
                        ImGui.Image(warningImage.ImGuiHandle, new Vector2(warningImage.Width, warningImage.Height));
                    }
                }
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
