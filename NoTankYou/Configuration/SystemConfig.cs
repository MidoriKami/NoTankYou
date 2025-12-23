using KamiLib.Configuration;

namespace NoTankYou.Configuration;

public class SystemConfig : CharacterConfiguration {
	public bool WaitUntilDutyStart = true;
	public bool HideInQuestEvent = true;
	
	public static SystemConfig Load()
		=> Utilities.Config.LoadCharacterConfig<SystemConfig>("System.config.json");
    
	public void Save()
		=> Utilities.Config.SaveCharacterConfig(this, "System.config.json"); 
}