namespace NoTankYou.Configuration.Components;

public class GenericSettings
{
    public Setting<bool> Enabled = new(false);
    public Setting<bool> SoloMode = new(false);
    public Setting<int> Priority = new(1);
    public Setting<bool> DutiesOnly = new(false);
    public Setting<bool> PartyFrameOverlay = new(true);
    public Setting<bool> BannerOverlay = new(true);
    public Setting<bool> DisableInSanctuary = new(true);
}
