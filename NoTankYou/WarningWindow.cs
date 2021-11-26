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
    class WarningWindow : Window, IDisposable
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

        private readonly Vector2 WindowSize = new Vector2(500, 100);

        private uint[] AllianceRaidTerritoryTypes;

        private bool IsAllianceRaid = false;

        private bool pauseDisplay = false;

        private int lastPartyCount = 0;
        private List<PartyMember> tankList;

        public WarningWindow(ImGuiScene.TextureWrap warningImage) : 
            base("Tank Stance Warning Window")
        {
            this.warningImage = warningImage;

            Service.ClientState.TerritoryChanged += OnTerritoryChanged;
            Service.Configuration.PluginPaused = false;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(WindowSize.X, WindowSize.Y),
                MaximumSize = new(WindowSize.X, WindowSize.Y)
            };

            IsOpen = true;

            InitalizeAllianceRaidTerritoryTypeList();
        }

        private void InitalizeAllianceRaidTerritoryTypeList()
        {
            var territorySheets = Service.DataManager.GetExcelSheet<TerritoryType>();

            if (territorySheets != null)
            {
                AllianceRaidTerritoryTypes = territorySheets.Where(r => r.TerritoryIntendedUse == 8).Select(r => r.RowId).ToArray();
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

        // Event triggers on map change
        // On map change, we want to hide the banner for "InstanceLoadDelayTime" milliseconds
        // Additionally we want to check if we entered an alliance raid
        private void OnTerritoryChanged(object? s, ushort u)
        {
            IsAllianceRaid = AllianceRaidTerritoryTypes.Contains(u);

            Service.Chat.Print($"Territory Changed. NewID:{u}");

            Service.Configuration.PluginPaused = false;
            IsOpen = true;
            pauseDisplay = true;
            Task.Delay(Service.Configuration.InstanceLoadDelayTime).ContinueWith(t => { pauseDisplay = false; });
        }

        public void Dispose()
        {
            warningImage.Dispose();
            Service.ClientState.TerritoryChanged -= OnTerritoryChanged;
        }

        // If we need to move the window around, we have to enable clickthrough
        public override void PreDraw()
        {
            base.PreDraw();
            ImGuiWindowFlags windowflags = Service.Configuration.EnableClickthrough ?  clickthroughFlags : defaultWindowFlags;

            Flags = windowflags;
        }
        public override void Draw()
        {
            // If force show banner is enabled, show it no matter what
            if (Service.Configuration.ForceShowNoTankWarning)
            {
                ImGui.Image(warningImage.ImGuiHandle, new Vector2(warningImage.Width, warningImage.Height));
            }

            // Else if this window is disabled exit
            else if (!Service.Configuration.ShowNoTankWarning && IsOpen)
            {
                return;
            }

            else if (Service.Configuration.PluginPaused)
            {
                return;
            }

            // Else if we are in an alliance raid, and we want to hide for an alliance raid
            else if (IsAllianceRaid && Service.Configuration.DisableInAllianceRaid)
            {
                return;
            }

            else if (Service.Configuration.TerritoryBlacklist.Contains(Service.ClientState.TerritoryType))
            {
                return;
            }

            else
            { 
                // If we aren't waiting for the loading screen to complete
                if (!pauseDisplay)
                {
                    // Check for tank stance, if one isn't found show warning banner
                    CheckForTankStanceAndDraw();
                }
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            IsOpen = true;
        }
    }
}
