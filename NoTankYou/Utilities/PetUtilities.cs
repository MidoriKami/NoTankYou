using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.ClientState.Statuses;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DisplaySystem;

namespace NoTankYou.Utilities
{
    internal static class PetUtilities
    {
        public static IEnumerable<GameObject> GetPetsByOwnerId(uint ownerId)
        {
            return Service.ObjectTable
                .Where(o => o.OwnerId == ownerId)
                .Where(o => o.ObjectKind is ObjectKind.BattleNpc && ((BattleNpc)o).SubKind == (int)BattleNpcSubKind.Pet);
        }

        public static Dictionary<PartyMember, Tuple<List<GameObject>, StatusList>> GetPartyMemberData()
        {
            Dictionary<PartyMember, Tuple<List<GameObject>, StatusList>> data = new();

            // Get Scholars
            var scholars = Service.PartyList.Where(p => p.ClassJob.Id is 28 && WarningBanner.IsTargetable(p));

            foreach (var scholar in scholars)
            {
                // Get this scholars pets
                var playerPets = GetPetsByOwnerId(scholar.ObjectId).ToList();

                var statuses = scholar.Statuses;

                data.Add(scholar, new Tuple<List<GameObject>, StatusList>(playerPets, statuses));
            }

            return data;
        }
        public static bool DelayMilliseconds(int milliseconds, Stopwatch stopwatch)
        {
            if (stopwatch.IsRunning == false)
            {
                stopwatch.Start();
                return false;
            }

            if (stopwatch.ElapsedMilliseconds < milliseconds && stopwatch.IsRunning)
            {
                return false;
            }

            stopwatch.Stop();
            stopwatch.Reset();
            return true;
        }
    }
}
