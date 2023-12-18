using System;
using R2API;
using RoR2;
using UnityEngine.Networking;

namespace Quo_Gin.Componenets
{
    internal class SunShotMark : NetworkBehaviour, IOnKilledServerReceiver
    {
        private SkillLocator SkillLocator;

        private void Start()
        {
            this.SkillLocator = base.GetComponent<SkillLocator>();
        }
        public void OnKilledServer(DamageReport damageReport)
        {
            bool flag = DamageAPI.HasModdedDamageType(damageReport.damageInfo, Quo_GinPlugin.SunShotMark) && NetworkServer.active;
            if (flag) 
            {
                GlobalEventManager.ProcIgniteOnKill(damageReport, 2, damageReport.victimBody,damageReport.attackerTeamIndex);
            }
        }

    }
}
