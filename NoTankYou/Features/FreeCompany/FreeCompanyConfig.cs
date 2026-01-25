using System.Text.Json.Serialization;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.FreeCompany;

public class FreeCompanyConfig : ConfigBase {
    public FreeCompanyMode Mode = FreeCompanyMode.Any;
    public uint PrimaryBuff;
    public uint SecondaryBuff;
    
    [JsonIgnore] public int BuffCount => this switch {
        { PrimaryBuff: not 0, SecondaryBuff: 0 } => 1,
        { PrimaryBuff: 0, SecondaryBuff: not 0 } => 1,
        { PrimaryBuff: not 0, SecondaryBuff: not 0 } => 2,
        _ => 0,
    };
}
