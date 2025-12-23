using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using KamiToolKit.Classes;
using Newtonsoft.Json;

namespace NoTankYou.Configuration;

public class BannerListStyle {
	[JsonIgnore] public bool EnableMoving = false;
	[JsonIgnore] public bool EnableResizing = false;

	public Vector2 Position = new(700.0f, 400.0f);
	public Vector2 Size = new(448.0f, 320.0f);
	public Vector2 Scale = Vector2.One;
	public LayoutOrientation Orientation = LayoutOrientation.Vertical;
	public LayoutAnchor Anchor = LayoutAnchor.TopLeft;
	public bool ShowBackground = false;
	public Vector4 BackgroundColor = KnownColor.LightBlue.Vector();
	
	public static BannerListStyle Load()
		=> Utilities.Config.LoadCharacterConfig<BannerListStyle>("BannerList.style.json");
    
	public void Save()
		=> Utilities.Config.SaveCharacterConfig(this, "BannerList.style.json"); 
}