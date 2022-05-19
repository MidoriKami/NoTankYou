using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoTankYou.Data.Components;

namespace NoTankYou.Data
{
    public class SystemSettings
    {
        public BlacklistSettings Blacklist { get; set; } = new();
    }
}
