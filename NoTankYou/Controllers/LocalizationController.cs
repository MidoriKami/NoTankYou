﻿using System;
using System.Globalization;
using KamiLib.Extensions;
using NoTankYou.Localization;

namespace NoTankYou.Controllers;

public class LocalizationController : IDisposable {
    public LocalizationController() {
        OnLanguageChange(Service.PluginInterface.UiLanguage);
        Service.PluginInterface.LanguageChanged += OnLanguageChange;

        EnumExtensions.GetCultureInfoFunc = () => Strings.Culture;
        EnumExtensions.GetResourceManagerFunc = () => Strings.ResourceManager;
    }
    
    public void Dispose()
        => Service.PluginInterface.LanguageChanged -= OnLanguageChange;

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