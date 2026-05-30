using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using NoTankYou.Enums;

namespace NoTankYou.Classes;

public class ModuleManager : IAsyncDisposable {

    public List<LoadedModule>? LoadedModules { get; private set; }

    private List<ModuleBase>? warningGeneratingModules;
    private List<FeatureBase>? warningDisplayingModules;

    public async ValueTask DisposeAsync()
        => await UnloadModules();

    public async Task LoadModules() {
        var allModules = GetModuleTypes();
        LoadedModules = [];
        warningGeneratingModules = [];
        warningDisplayingModules = [];

        List<Task> loadTasks = [];

        foreach (var module in allModules.OrderBy(module => module.ModuleInfo.Type).ThenBy(module => module.Name)) {
            Services.PluginInterface.Inject(module);

            var newLoadedModule = new LoadedModule(module, LoadedState.Disabled);

            LoadedModules.Add(newLoadedModule);

            loadTasks.Add(Task.Run(async () => {
                await module.Load();

                if (System.SystemConfig?.EnabledModules.Contains(module.Name) ?? false) {
                    await TryEnableModule(newLoadedModule);
                }
            }));

            if (module is ModuleBase moduleBase) {
                warningGeneratingModules.Add(moduleBase);
            }
            else {
                warningDisplayingModules.Add(module);
            }
        }

        await Task.WhenAll(loadTasks);

        LoadedModules.ToFrozenDictionary(module => module.Name, module => module);

        Services.Framework.Update += OnFrameworkUpdate;
    }

    public async Task UnloadModules() {
        if (LoadedModules is null) {
            Services.PluginLog.Debug("No modules loaded");
            return;
        }

        Services.PluginLog.Debug("Disposing Module Manager, now disabling all Modules");
        Services.Framework.Update -= OnFrameworkUpdate;

        List<Task> unloadTasks = [];

        foreach (var loadedModule in LoadedModules) {
            unloadTasks.Add(Task.Run(async () => {
                if (loadedModule.State is LoadedState.Enabled) {
                    try {
                        Services.PluginLog.Info($"Disabling {loadedModule.Name}");
                        await loadedModule.FeatureBase.Disable();
                        Services.PluginLog.Info($"Successfully Disabled {loadedModule.Name}");
                    }
                    catch (Exception e) {
                        Services.PluginLog.Error(e, $"Error while unloading modification {loadedModule.Name}");
                    }
                }

                await loadedModule.FeatureBase.Unload();
            }));
        }

        await Task.WhenAll(unloadTasks);

        LoadedModules = null;
    }

    public static async Task TryEnableModule(LoadedModule module) {
        if (System.SystemConfig is null) {
            Services.PluginLog.Error("System Config Failed to Load.");
            return;
        }

        if (module.State is LoadedState.Errored) {
            Services.PluginLog.Error($"[{module.Name}] Attempted to enable errored module");
            return;
        }

        try {
            Services.PluginLog.Info($"Enabling {module.Name}");
            await module.FeatureBase.Enable();
            module.State = LoadedState.Enabled;
            Services.PluginLog.Info($"Successfully Enabled {module.Name}");

            if (System.SystemConfig.EnabledModules.Add(module.Name)) {
                await System.SystemConfig.Save();
            }
        }
        catch (Exception e) {
            module.State = LoadedState.Errored;
            module.ErrorMessage = "Failed to load, this module has been disabled.";
            Services.PluginLog.Error(e, $"Error while enabling {module.Name}, attempting to disable");

            try {
                await module.FeatureBase.Disable();
                Services.PluginLog.Information($"Successfully disabled erroring module {module.Name}");
            }
            catch (Exception fatal) {
                module.ErrorMessage = "Critical Error: Module failed to load, and errored again while unloading.";
                Services.PluginLog.Error(fatal, $"Critical Error while trying to unload erroring module: {module.Name}");
            }
        }
    }

    public static async Task TryDisableModification(LoadedModule modification, bool removeFromList = true) {
        if (System.SystemConfig is null) {
            Services.PluginLog.Error("System Config Failed to Load.");
            return;
        }

        if (modification.State is LoadedState.Errored) {
            Services.PluginLog.Error($"[{modification.Name}] Attempted to disable errored modification");
            return;
        }

        try {
            Services.PluginLog.Info($"Disabling {modification.Name}");
            await modification.FeatureBase.Disable();
            modification.FeatureBase.OpenConfigAction = null;
        }
        catch (Exception e) {
            modification.State = LoadedState.Errored;
            Services.PluginLog.Error(e, $"Failed to Disable {modification.Name}");
        } finally {
            modification.State = LoadedState.Disabled;
            Services.PluginLog.Debug($"Successfully Disabled {modification.Name}");

            if (removeFromList) {
                System.SystemConfig.EnabledModules.Remove(modification.Name);
                await System.SystemConfig.Save();
            }
        }
    }

    private void OnFrameworkUpdate(IFramework framework) {
        foreach (var module in warningGeneratingModules ?? []) {
            module.Update();
        }

        System.WarningController.CollectWarnings(LoadedModules?.Select(module => module.FeatureBase).OfType<ModuleBase>());

        foreach (var module in warningDisplayingModules ?? []) {
            module.Update();
        }
    }

    private static List<FeatureBase> GetModuleTypes() => Assembly
        .GetCallingAssembly()
        .GetTypes()
        .Where(type => type.IsSubclassOf(typeof(FeatureBase)))
        .Where(type => !type.IsAbstract)
        .Select(type => (FeatureBase?)Activator.CreateInstance(type))
        .Where(modification => modification?.ModuleInfo.Type is not ModuleType.Hidden)
        .OfType<FeatureBase>()
        .ToList();
}
