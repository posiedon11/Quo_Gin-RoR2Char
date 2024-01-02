using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine.Networking;


namespace Quo_Gin.Componenets
{
    internal class AscendingDawnBuff : NetworkBehaviour
    {

        public void Awake()
        {
            this.Hook_AscendingDawn();
        }
        public void Hook_AscendingDawn()
        {
            Log.Debug("Hooking Ascending Dawn");
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
        }
        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);

            if (damageReport.victimBody)
            {
                if (damageReport.victimBody.HasBuff(Quo_Gin.Modules.Buffs.ascendingDawnJumpBuff))
                {
                    //GlobalEventManager.ProcIgniteOnKill(damageReport, 5, damageReport.victimBody, damageReport.attackerTeamIndex);
                }
            }

            if(damageReport.attackerBody.HasBuff(Modules.Buffs.ascendingDawnJumpBuff))
            {
                Log.Debug("Granding Move Speed Buff");
                damageReport.attackerBody.AddTimedBuff(Modules.Buffs.ascendingDawnKillBuff, SkillStates.AscendingDawn.killSpeedDuration);
            }
        }

    }
}
