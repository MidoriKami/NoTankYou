using System;
using System.Collections.Generic;
using System.Linq;
using NoTankYou.Interfaces;
using NoTankYou.Windows.BannerOverlayWindow;
using NoTankYou.Windows.NoTankYouWindow;
using NoTankYou.Windows.PartyFrameOverlayWindow;

namespace NoTankYou.System
{
    public class WindowManager : IDisposable
    {
        private readonly List<IDisposable> WindowList = new()
        {
            new NoTankYouWindow(),
            new PartyFrameOverlayWindow(),
            new BannerOverlayWindow(),
        };

        public void Dispose()
        {
            foreach (var window in WindowList)
            {
                window.Dispose();
            }
        }

        public T? GetWindowOfType<T>()
        {
            return WindowList.OfType<T>().FirstOrDefault();
        }

        public void ExecuteCommand(string command, string arguments)
        {
            foreach (var eachCommand in WindowList.OfType<ICommand>())
            {
                eachCommand.ProcessCommand(command, arguments);
            }
        }
    }
}
