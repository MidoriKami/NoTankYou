using System;
using System.Numerics;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using NoTankYou.Components;
using NoTankYou.Interfaces;
using NoTankYou.Utilities;

namespace NoTankYou.Windows.NoTankYouWindow
{
    internal class NoTankYouWindow : Window, ICommand, IDisposable
    {
        private readonly SelectionPane SelectionPane = new()
        {
            Padding = 6.0f,
            SelectionPaneWidth = 300.0f
        };

        public NoTankYouWindow() : base("NoTankYou Settings")
        {
            Service.WindowSystem.AddWindow(this);

            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(775, 400),
                MaximumSize = new Vector2(9999,9999)
            };

            Flags |= ImGuiWindowFlags.NoScrollbar;
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        }

        public void Dispose()
        {
            Service.WindowSystem.RemoveWindow(this);
        }

        public override void PreOpenCheck()
        {
            if (Service.ClientState.IsPvP)
                IsOpen = false;
        }

        public override void PreDraw()
        {

        }

        public override void Draw()
        {
            SelectionPane.Draw();
        }

        public override void PostDraw()
        {

        }

        public override void OnClose()
        {
            Service.PluginInterface.UiBuilder.AddNotification("Settings Saved", "No Tank You", NotificationType.Success);

            Service.Configuration.Save();
        }

        void ICommand.Execute(string? primaryCommand, string? secondaryCommand)
        {
            switch (primaryCommand)
            {
                case null:
                    Toggle();
                    break;

                case "supersecretpassword" when !Service.Configuration.DeveloperMode:
                    Chat.Debug("Password Accepted - Admin Mode Enabled");
                    Service.Configuration.DeveloperMode = true;
                    Service.Configuration.Save();
                    break;                
                
                case "supersecretpassword" when Service.Configuration.DeveloperMode:
                    Chat.Debug("Admin Mode Already Enabled");
                    break;

                case "goodnight" when Service.Configuration.DeveloperMode:
                    Chat.Debug("Good Night - Admin Mode Disabled");
                    Service.Configuration.DeveloperMode = false;
                    Service.Configuration.Save();
                    break;

            }
        }
    }
}
