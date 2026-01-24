using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;
using NoTankYou.CustomNodes;
using NoTankYou.Enums;

namespace NoTankYou.Features.PartyList.ConfigurationCategories;

public class ModuleSelectConfigNode : VerticalListNode {
    private readonly PartyList module;

    public ModuleSelectConfigNode(PartyList module) {
        this.module = module;
        FitWidth = true;
        ItemSpacing = 3.0f;
        FitContents = true;
        
        InitialNodes = [
            new CategoryHeaderNode {
                String= "Module Blacklist",
                Alignment = AlignmentType.Bottom,
                TextTooltip = "Warnings for the selected modules will not be shown in the Party List overlay.",
            },
            ..GetModuleCheckBoxes(),
        ];
    }
    
    private IEnumerable<NodeBase> GetModuleCheckBoxes() {
        foreach (var moduleName in AllModules) {
            yield return new CheckboxNode {
                Height = 32.0f,
                String = moduleName,
                IsChecked = module.Config.BlacklistedModules.Contains(moduleName),
                OnClick = newValue => {
                    if (newValue) {
                        if (!module.Config.BlacklistedModules.Contains(moduleName)) {
                            module.Config.BlacklistedModules.Add(moduleName);
                        }
                    }
                    else {
                        module.Config.BlacklistedModules.Remove(moduleName);
                    }
                    module.Config.MarkDirty();
                },
            };
        }
    }
    
    private static List<string> AllModules 
        => System.ModuleManager.LoadedModules?
              .Where(loadedModule => loadedModule.FeatureBase.ModuleInfo.Type is not ModuleType.GeneralFeatures)
              .Select(loadedModule => loadedModule.Name)
              .ToList() ?? [];
}
