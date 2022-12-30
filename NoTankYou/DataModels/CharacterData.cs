using System;

namespace NoTankYou.DataModels;

[Serializable]
public record CharacterData
{
    public string Name = "Unknown";
    public ulong LocalContentID = 0;
}