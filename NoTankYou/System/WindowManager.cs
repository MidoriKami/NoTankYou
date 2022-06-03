using System;
using System.Collections.Generic;
using System.Linq;
using NoTankYou.Interfaces;
using NoTankYou.Windows.DisplayModules;
using NoTankYou.Windows.NoTankYouWindow;

namespace NoTankYou.System
{
    public class WindowManager : IDisposable
    {
        private readonly List<IDisposable> DisplayModule = new()
        {
            new NoTankYouWindow(),
            new PartyFrameOverlayWindow(),
            new BannerOverlayWindow(),
            new TippyOverlay()
        };

        public void Dispose()
        {
            foreach (var module in DisplayModule)
            {
                module.Dispose();
            }
        }

        public T? GetWindowOfType<T>()
        {
            return DisplayModule.OfType<T>().FirstOrDefault();
        }

        public void ExecuteCommand(string command, string arguments)
        {
            foreach (var eachCommand in DisplayModule.OfType<ICommand>())
            {
                eachCommand.ProcessCommand(command, arguments);
            }
        }
    }
}
