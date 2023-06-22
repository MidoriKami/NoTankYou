using System;
using System.Collections.Generic;
using System.Linq;
using KamiLib.AutomaticUserInterface;
using NoTankYou.DataModels;
using NoTankYou.Models;
using NoTankYou.Views.Components;

namespace NoTankYou.System;

public class PartyListController : IDisposable
{
    private PartyListConfig config = new();
    
    private readonly PartyMemberOverlay?[] partyMembers = new PartyMemberOverlay[8];

    private static WarningState SampleWarning => new()
    {
        Message = "NoTankYou Sample Warning",
        Priority = 100,
        IconId = 786,
        IconLabel = "Sample Action",
        SourceObjectId = Service.ClientState.LocalPlayer?.ObjectId ?? 0xE000000,
        SourcePlayerName = "Sample Player",
    };

    public void Update()
    {
        foreach (var member in partyMembers)
        {
            member?.Update();
        }
    }

    public void Draw(List<WarningState> warnings)
    {
        if (!config.Enabled) return;

        if (config.SampleMode)
        {
            partyMembers[0]?.DrawWarning(SampleWarning);
            return;
        }

        if (config.SoloMode)
        {
            var warning = warnings
                .Where(warning => warning.SourceObjectId == Service.ClientState.LocalPlayer?.ObjectId)
                .MaxBy(warning => warning.Priority);
            
            partyMembers[0]?.DrawWarning(warning);
        }
        else
        {
            foreach (var partyMember in partyMembers)
            {
                var warning = warnings
                    .Where(warning => warning.SourceObjectId == partyMember?.ObjectId)
                    .MaxBy(warning => warning.Priority);
            
                partyMember?.DrawWarning(warning);
            }
        }
    }

    public void Load()
    {
        config = LoadConfig();

        foreach (var index in Enumerable.Range(0, 8))
        {
            partyMembers[index] = new PartyMemberOverlay(config, index);
        }
    }

    public void Unload()
    {
        foreach (var member in partyMembers)
        {
            member?.Reset(true);
        }
    }

    public void Dispose() => Unload();
    public void DrawConfig() => DrawableAttribute.DrawAttributes(config, SaveConfig);
    private PartyListConfig LoadConfig() => FileController.LoadFile<PartyListConfig>("PartyListOverlay.config.json", config);
    public void SaveConfig() => FileController.SaveFile("PartyListOverlay.config.json", config.GetType(), config);
}