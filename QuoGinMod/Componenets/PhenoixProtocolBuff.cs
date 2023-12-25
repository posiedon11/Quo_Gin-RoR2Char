using Quo_Gin.SkillStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using UnityEngine.Networking;

namespace Quo_Gin.Componenets
{
    internal class PhoenixProtocolBuff : NetworkBehaviour, IOnKilledOtherServerReceiver
    {
        private SkillLocator skillLocator;

        internal const float minimumHealthForProFraction = 2f;
        internal const float skillCoolDownForSelf = 3f;
        private void Start()
        {
            Log.Debug("Starting Passive Skill");
            this.skillLocator = base.GetComponent<SkillLocator>();
        }

        public void OnKilledOtherServer(DamageReport damageReport)
        {
            if (NetworkServer.active)
            {
                Log.Debug("Something Died");
                CharacterBody attackerBody = damageReport.attacker.GetComponent<CharacterBody>();
                if (base.hasAuthority)
                {
                    Log.Debug($"{attackerBody.name} Killed it");
                    
                    if (attackerBody.HasBuff(Modules.Buffs.wellOfRadianceBuff))
                    {
                        
                        Log.Debug("Something died while having buff");
                        if (attackerBody.healthComponent.GetHealthBarValues().healthFraction < minimumHealthForProFraction)
                        {
                            Log.Debug($"{base.GetComponent<CharacterBody>().name} is getting cooldown");
                            this.skillLocator.special.rechargeStopwatch += skillCoolDownForSelf;
                            this.skillLocator.utility.rechargeStopwatch += skillCoolDownForSelf;
                            this.skillLocator.primary.rechargeStopwatch += skillCoolDownForSelf;
                            this.skillLocator.secondary.rechargeStopwatch += skillCoolDownForSelf;
                        }
                    }
                }
            }
        }


    }
}
