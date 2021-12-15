using System;

namespace NoTankYou.DisplaySystem
{
    internal interface IWarningBanner : IDisposable
    {
        public bool Visible { get; set; }
        public bool Paused { get; set; }

        public void Update();
    }
}
