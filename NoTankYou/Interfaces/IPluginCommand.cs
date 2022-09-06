namespace NoTankYou.Interfaces;

internal interface IPluginCommand
{
    string? CommandArgument { get; }

    void Execute(string? additionalArguments);
}