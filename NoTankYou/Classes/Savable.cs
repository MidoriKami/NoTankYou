using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;

namespace NoTankYou.Classes;

public abstract class Savable {
    [JsonIgnore] public string FileName { get; set; } = string.Empty;
    [JsonIgnore] public bool SavePending;

    protected abstract string FileExtension { get; }

    public void MarkDirty()
        => SavePending = true;

    public virtual async Task Save() {
        SavePending = false;

        if (FileName == string.Empty) {
            IPluginLog.Get().Error("Tried to save a config with no file name set");
            return;
        }

        IPluginLog.Get().Debug($"Saving {FileName}{FileExtension}");
        await Utilities.Config.SaveCharacterConfig(this, $"{FileName}{FileExtension}");
    }
}
