using System;
using System.Numerics;
using System.Threading.Tasks;
using ImGuiNET;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using Lumina.Excel.GeneratedSheets;
using System.Linq;

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

        public WarningWindow(ImGuiScene.TextureWrap warningImage) : 
            base("Tank Stance Warning Window")
        {
            this.warningImage = warningImage;

            Service.ClientState.TerritoryChanged += TriggerRenderDelay;

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

        // Checks all party members for a tank stance then displays the banner
        public void CheckForTankStanceAndDraw()
        {
            // Is the player in a party? and also in a duty?
            if (PartyOperations.IsInParty(Service.PartyList) && Service.Condition[ConditionFlag.BoundByDuty])
            {
                // Is there a tank present in the party? (often false for things like Palace of the Dead)
                var partyTanks = PartyOperations.GetTanksList(Service.PartyList);

                // If we found any tanks
                if (partyTanks.Count > 0)
                {
                    bool tankStanceFound = false;

                    // Check each tank for a tank stance
                    foreach(var tank in partyTanks)
                    {
                        if (PartyOperations.IsTankStanceFound(tank))
                        {
                            tankStanceFound = true;
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

        // On map change, we want to hide the banner for "InstanceLoadDelayTime" milliseconds
        private void TriggerRenderDelay(object? s, ushort u)
        {
            IsAllianceRaid = AllianceRaidTerritoryTypes.Contains(u);

            IsOpen = true;
            pauseDisplay = true;
            Task.Delay(Service.Configuration.InstanceLoadDelayTime).ContinueWith(t => { pauseDisplay = false; });
        }

        public void Dispose()
        {
            warningImage.Dispose();
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

            // Else if we are in an alliance raid, and we want to hide for an alliance raid
            if(IsAllianceRaid && Service.Configuration.DisableInAllianceRaid)
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
