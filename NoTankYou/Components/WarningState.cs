using NoTankYou.Enums;

namespace NoTankYou.Components
{
    public class WarningState
    {
        public string IconLabel { get; set; } = string.Empty;
        public uint IconID { get; set; }
        public string MessageLong { get; set; } = string.Empty;
        public string MessageShort { get; set; } = string.Empty;
        public int Priority { get; set; }
        public ModuleType Sender { get; set; }
        public SenderSubtype SenderSubtype { get; set; } 
    }
}
