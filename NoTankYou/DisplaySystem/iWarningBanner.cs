using Dalamud.Interface.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoTankYou.DisplaySystem
{
    internal interface IWarningBanner : IDisposable
    {
        public bool Visible { get; set; }
        public bool Paused { get; set; }

        public void Update();
    }
}
