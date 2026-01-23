using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;

namespace NoTankYou.Extensions;

public static unsafe class BattleCharaExtensions {
    extension(ref BattleChara battleChara) {
        public bool IsTank 
            => Services.DataManager.GetExcelSheet<ClassJob>().GetRow(battleChara.ClassJob).Role is 1;

        public int? HudIndex 
            => AgentHUD.Instance()->GetMember(battleChara.EntityId)?.Index;

        public bool MissingStatus(ICollection<uint> statuses) {
            foreach (var status in statuses) {
                if (battleChara.StatusManager.HasStatus(status)) return false;
            }

            return true;
        }

        public BattleChara* Pet {
            get {
                foreach (var characterEntry in CharacterManager.Instance()->BattleCharas) {
                    if (characterEntry.Value is null) continue;
                    if (characterEntry.Value->OwnerId == battleChara.EntityId) 
                        return characterEntry.Value;
                }

                return null;
            }
        }
    }
}
