namespace NoTankYou.Configuration;

public class BannerStyle {
	public bool ShowWarningIcon = true;
	public bool ShowMessageText = true;
	public bool ShowPlayerText = true;
	public bool ShowActionName = true;
	public bool ShowActionIcon = true;
	public bool EnableAnimation = true;
	public bool EnableActionTooltip = true;
	
	public static BannerStyle Load()
		=> Utilities.Config.LoadCharacterConfig<BannerStyle>("BannerNode.style.json");
    
	public void Save()
		=> Utilities.Config.SaveCharacterConfig(this, "BannerNode.style.json"); 
}