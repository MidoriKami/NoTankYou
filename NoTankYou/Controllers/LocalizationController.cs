using System;
using System.Globalization;
using NoTankYou.Localization;

namespace NoTankYou.Controllers;

public class LocalizationController : IDisposable {
    public void Initialize() {
        Strings.Culture = new CultureInfo(Service.PluginInterface.UiLanguage);

        Service.PluginInterface.LanguageChanged += OnLanguageChange;
    }
    
    public void Dispose() {
        Service.PluginInterface.LanguageChanged -= OnLanguageChange;
    }

    private void OnLanguageChange(string languageCode) {
        try {
            Service.Log.Information($"Loading Localization for {languageCode}");
            Strings.Culture = new CultureInfo(languageCode);
        }
        catch (Exception ex) {
            Service.Log.Error(ex, "Unable to Load Localization");
        }
    }
}