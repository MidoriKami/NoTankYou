using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace NoTankYou.Utilities;

public static unsafe class PartyListMemberStructExtensions
{
    public static void SetPlayerNameOutlineColor(this AddonPartyList.PartyListMemberStruct data, Vector4 color)
    {
        data.Name->AtkResNode.AddRed = (ushort)(color.X * 255);
        data.Name->AtkResNode.AddGreen = (ushort)(color.Y * 255);
        data.Name->AtkResNode.AddBlue = (ushort)(color.Z * 255);
    }

    public static void SetIconVisibility(this AddonPartyList.PartyListMemberStruct data, bool visible)
    {
        data.ClassJobIcon->AtkResNode.ToggleVisibility(visible);
    }

    public static Vector2 GetIconPosition(this AddonPartyList.PartyListMemberStruct data)
    {
        var parentOffset = data.ClassJobIcon->AtkResNode.ParentNode->Y;
        var jobIcon = data.ClassJobIcon->AtkResNode;
        var listOffset = data.PartyMemberComponent->OwnerNode->AtkResNode.ParentNode->ParentNode->Y;

        return new Vector2(jobIcon.X, jobIcon.Y + parentOffset + listOffset);
    }

    public static Vector2 GetIconSize(this AddonPartyList.PartyListMemberStruct data)
    {
        var jobIcon = data.ClassJobIcon->AtkResNode;

        return new Vector2(jobIcon.Width, jobIcon.Height);
    }

    public static Vector2 GetNamePosition(this AddonPartyList.PartyListMemberStruct data)
    {
        var nameElement = data.NameAndBarsContainer;
        var memberOffset = data.PartyMemberComponent->OwnerNode->AtkResNode.Y;
        var listOffset = data.PartyMemberComponent->OwnerNode->AtkResNode.ParentNode->ParentNode->Y;
        
        return new Vector2(nameElement->X, nameElement->Y + memberOffset + listOffset);
    }

    public static Vector2 GetNameSize(this AddonPartyList.PartyListMemberStruct data)
    {
        var nameElement = data.NameAndBarsContainer;

        return new Vector2(nameElement->Width, nameElement->Height);
    }
}