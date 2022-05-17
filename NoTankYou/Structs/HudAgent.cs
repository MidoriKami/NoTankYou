using System.Runtime.InteropServices;

namespace NoTankYou.Structs
{
    public class GroupMember
    {
        public long ContentId { get; set; }
        public int ObjectId { get; set; }
    }

    [StructLayout(LayoutKind.Explicit, Size = 0xDC8)]
    public unsafe struct HudAgent
    {
        [FieldOffset(0xCC8)] private fixed byte raw[8 * 0x20];

        public GroupMember Get(int index)
        {
            //Chat.Debug($"ContentID:{(IntPtr)(raw +}");

            return new()
            {
                ContentId = raw[index * 0x20 + 0x10],
                ObjectId = raw[index * 0x20 + 0x18]
            };
        }

    }
}
