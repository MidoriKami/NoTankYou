using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.ClientState.Conditions;
using System.Threading;
using Dalamud.Interface.Windowing;

namespace NoTankYou
{
    class WarningWindow : Window, IDisposable
    {
        private ImGuiScene.TextureWrap warningImage;

        private ImGuiWindowFlags defaultWindowFlags =
                    ImGuiWindowFlags.NoScrollbar |
                    ImGuiWindowFlags.NoScrollWithMouse |
                    ImGuiWindowFlags.NoTitleBar |
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoResize |
                    ImGuiWindowFlags.NoBackground;

        private ImGuiWindowFlags noClickthroughFlags = 
                    ImGuiWindowFlags.NoScrollbar |
                    ImGuiWindowFlags.NoScrollWithMouse |
                    ImGuiWindowFlags.NoTitleBar |
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoResize |
                    ImGuiWindowFlags.NoBackground |
                    ImGuiWindowFlags.NoMouseInputs;

        private bool pauseDisplay = false;
        private Vector2 WindowSize { get; set; }

        public WarningWindow(ImGuiScene.TextureWrap warningImage) : 
            base("Tank Stance Warning Window")
        {
            this.warningImage = warningImage;

            Service.ClientState.TerritoryChanged += TriggerRenderDelay;

            WindowSize = new Vector2(500, 100);

            this.SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(WindowSize.X, WindowSize.Y),
                MaximumSize = new(WindowSize.X, WindowSize.Y)
            };

            IsOpen = true;
        }

        // Checks all party members for a tank stance then displays the banner
        public void CheckForTankStanceAndDraw()
        {
            // If force show, will cause this window to always be displayed (unless render is being delayed)
            if (Service.Configuration.ForceShowNoTankWarning == true)
            {
                ImGui.Image(this.warningImage.ImGuiHandle, new Vector2(this.warningImage.Width, this.warningImage.Height));
            }

            // Is the player in a party? and also in a duty?
            else if (PartyOperations.IsInParty(Service.PartyList) && Service.Condition[ConditionFlag.BoundByDuty])
            {
                // Is there a tank present in the party? (often false for things like Palace of the Dead)
                var tankPresent = PartyOperations.IsTankPresent(Service.PartyList);

                // Item1 boolean, true if a tank was found, false if not
                if (tankPresent.Item1 == true)
                {
                    // Item2 PartyMember object, the first found tank
                    // Do they have a tank stance status?
                    if (tankPresent.Item2 != null && !PartyOperations.IsTankStanceFound(tankPresent.Item2))
                    {
                        // Display warning banner
                        ImGui.Image(this.warningImage.ImGuiHandle, new Vector2(this.warningImage.Width, this.warningImage.Height));
                    }
                }
            }
        }

        // On map change, we want to hide the banner for "InstanceLoadDelayTime" milliseconds
        private void TriggerRenderDelay(object? s, ushort u)
        {
            pauseDisplay = true;
            Task.Delay(Service.Configuration.InstanceLoadDelayTime).ContinueWith(t => { pauseDisplay = false; });
        }
        public void Dispose()
        {
            this.warningImage.Dispose();
        }

        // If we need to move the window around, we have to enable clickthrough
        public override void PreDraw()
        {
            base.PreDraw();
            ImGuiWindowFlags windowflags = Service.Configuration.DisableClickthrough ? noClickthroughFlags : defaultWindowFlags;

            Flags = windowflags;
        }
        public override void Draw()
        {
            if (!Service.Configuration.ShowNoTankWarning && IsOpen)
            {
                return;
            }
            else if (Service.PartyList.IsAlliance && Service.Configuration.DisableInAllianceRaids)
            {
                return;
            }
            else
            { 
                if (!pauseDisplay)
                {
                    CheckForTankStanceAndDraw();
                }
            }
        }
    }
}
