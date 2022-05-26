using NoTankYou.Data.Components;

namespace NoTankYou.Data
{
    public class SystemSettings
    {
        public BlacklistSettings Blacklist { get; set; } = new();
        public bool DisablePartyListVisibilityChecking = false;
    }
}
