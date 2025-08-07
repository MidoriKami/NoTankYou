﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using KamiLib.Window;
using Lumina.Excel.Sheets;

namespace NoTankYou.Windows;

public class FreeCompanyStatusSelectionWindow : SelectionWindowBase<Status> {
	protected override bool AllowMultiSelect => false;
	protected override float SelectionHeight => 32.0f * ImGuiHelpers.GlobalScale;

	public FreeCompanyStatusSelectionWindow() : base(new Vector2(300.0f, 500.0f)){
		SelectionOptions = Service.DataManager.GetExcelSheet<Status>().Where(status => status.IsFcBuff).ToList();
	}
	
	protected override void DrawSelection(Status option) {
		ImGui.Image(Service.TextureProvider.GetFromGameIcon(option.Icon).GetWrapOrEmpty().Handle, ImGuiHelpers.ScaledVector2(24.0f, 32.0f));
		
		ImGui.SameLine();
		ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 8.0f * ImGuiHelpers.GlobalScale);
		ImGui.Text(option.Name.ToString());
	}

	protected override IEnumerable<string> GetFilterStrings(Status option)
		=> [option.Name.ToString()];

	protected override string GetElementKey(Status element)
		=> element.RowId.ToString();
}