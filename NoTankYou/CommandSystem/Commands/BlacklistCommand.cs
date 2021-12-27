namespace NoTankYou.CommandSystem.Commands
{
    internal static class BlacklistCommand
    {
        public static void ProcessBlacklistCommand(string? secondaryCommand)
        {
            switch (secondaryCommand?.ToLower())
            {
                case null:
                case "here":
                    AddToBlacklist(Service.ClientState.TerritoryType);
                    break;

                case "add":
                    AddToBlacklist(secondaryCommand);
                    break;

                case "remove":
                    RemoveFromBlacklist(secondaryCommand);
                    break;

                default:
                    AddToBlacklist(secondaryCommand);
                    break;
            }
        }

        public static void ProcessWhitelistCommand(string? secondaryCommand)
        {
            switch (secondaryCommand?.ToLower())
            {
                case null:
                case "here":
                    RemoveFromBlacklist(Service.ClientState.TerritoryType);
                    break;

                case "add":
                    RemoveFromBlacklist(secondaryCommand);
                    break;

                case "remove":
                    AddToBlacklist(secondaryCommand);
                    break;

                default:
                    RemoveFromBlacklist(secondaryCommand);
                    break;
            }
        }

        private static void RemoveFromBlacklist(string command)
        {
            if (int.TryParse(command, out var number))
            {
                RemoveFromBlacklist(number);
            }
        }

        private static void RemoveFromBlacklist(int territory)
        {
            var blacklist = Service.Configuration.TerritoryBlacklist;

            if (blacklist.Contains(territory))
            {
                blacklist.Remove(territory);
                Service.Configuration.ForceWindowUpdate = true;
            }
        }

        private static void AddToBlacklist(string command)
        {
            if (int.TryParse(command, out var number))
            {
                AddToBlacklist(number);
            }
        }

        private static void AddToBlacklist(int territory)
        {
            var blacklist = Service.Configuration.TerritoryBlacklist;

            if (!blacklist.Contains(territory))
            {
                blacklist.Add(territory);
                Service.Configuration.ForceWindowUpdate = true;
            }
        }
    }
}
