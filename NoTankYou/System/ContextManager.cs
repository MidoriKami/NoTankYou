using System;
using Dalamud.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace NoTankYou.System;

public unsafe class ContextManager : IDisposable
{
    public bool ShowWarnings { get; private set; }

    public ContextManager()
    {
        Service.Framework.Update += FrameworkUpdate;
    }
        
    public void Dispose()
    {
        Service.Framework.Update -= FrameworkUpdate;
    }

    private void FrameworkUpdate(Framework framework)
    {
        var partyList = (AtkUnitBase*) Service.GameGui.GetAddonByName("_PartyList", 1);
        var todoList = (AtkUnitBase*) Service.GameGui.GetAddonByName("_ToDoList", 1);
        var enemyList = (AtkUnitBase*) Service.GameGui.GetAddonByName("_EnemyList", 1);

        var partyListVisible = partyList != null && partyList->IsVisible;
        var todoListVisible = todoList != null && todoList->IsVisible;
        var enemyListVisible = enemyList != null && enemyList->IsVisible;

        if (!partyListVisible && !todoListVisible && !enemyListVisible)
        {
            ShowWarnings = false;
        }
        else
        {
            ShowWarnings = true;
        }
    }
}