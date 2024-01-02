using System;
using R2API;
using RoR2;
using UnityEngine.Networking;

namespace Quo_Gin.Componenets
{
    internal class SunShotMark : NetworkBehaviour, IOnKilledServerReceiver
    {
        private SkillLocator SkillLocator;
        private void Awake()
        {
            this.Hook_SunShot();
        }
        private void Start()
        {
            this.SkillLocator = base.GetComponent<SkillLocator>();
        }
        public void OnKilledServer(DamageReport damageReport)
        {
            bool flag = DamageAPI.HasModdedDamageType(damageReport.damageInfo, Quo_GinPlugin.SunShotMark) && NetworkServer.active;
            if (flag)
            {
                GlobalEventManager.ProcIgniteOnKill(damageReport, 2, damageReport.victimBody, damageReport.attackerTeamIndex);
            }
        }
        public void Hook_SunShot()
        {
            Log.Debug("Hooking SunShot");
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self.body.HasBuff(Quo_Gin.Modules.Buffs.sunShotDebuff))
            {
                damageInfo.damage *= 3.5f;
            }
            orig(self, damageInfo);

            bool hasSunshotMark = DamageAPI.HasModdedDamageType(damageInfo, Quo_GinPlugin.SunShotMark);

            if (hasSunshotMark)
            {
                self.body.AddTimedBuff(Quo_Gin.Modules.Buffs.sunShotDebuff, 7);
            }
        }
    }
}
