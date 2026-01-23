using System.Text.RegularExpressions;
using NoTankYou.Enums;
using NoTankYou.Extensions;

namespace NoTankYou.Classes;

public class ModuleInfo {
    public required string DisplayName { get; init; }
	public required ModuleType Type { get; init; }
    public required string FileName { get; init; }
    public required uint IconId { get; init; }

    public bool IsMatch(string searchTerm) {
        var regex = new Regex(searchTerm, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        
        if (regex.IsMatch(DisplayName)) return true;
        if (regex.IsMatch(Type.Description)) return true;
        
        return false;
    }
}
