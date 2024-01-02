using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.SendMouseEvents;

namespace Quo_Gin.Componenets
{
    internal class ScorchDebuff : NetworkBehaviour //, IOnIncomingDamageServerReceiver, IOnTakeDamageServerReceiver
    {
        public static float scorchStacksToExplode = 3f;
        private static float damagePerStack = .25f;
        public static float scorchExplosionCoefficient = 2f;
        private static float scorchExplosionRadius = 20f;
        public static float scorchDuration = 8f;

        public void Awake()
        {
            this.Hook_ScorchDebuff();
        }
        public void Start()
        {
            Log.Debug("Starting Scorch Debuff");
        }
        //public void OnIncomingDamageServer(DamageInfo damageInfo)
        //{
        //    CharacterBody victimBody = this.GetComponent<CharacterBody>();
        //    Log.Debug("Damage Taken:");
        //    if ( DamageAPI.HasModdedDamageType(damageInfo, Quo_GinPlugin.ScorchMark))
        //    {
        //        Log.Debug("Add Scorch Debuff");
        //        victimBody.AddBuff(Modules.Buffs.scorchDebuff);
        //    }

        //    if (victimBody.HasBuff(Modules.Buffs.scorchDebuff))
        //    {
        //        damageInfo.damage *= damagePerStack * victimBody.GetBuffCount(Modules.Buffs.scorchDebuff);
        //    }
        //}

        //public void OnTakeDamageServer(DamageReport damageReport)
        //{
        //    CharacterBody victimBody = damageReport.victimBody;
        //    if (victimBody.GetBuffCount(Modules.Buffs.scorchDebuff) > scorchStacksToExplode)
        //    {
        //        Log.Debug("Scorch Explosion");
        //        BlastAttack blastAttack = new BlastAttack()
        //        {
        //            baseDamage = damageReport.damageDealt * scorchExplosionCoefficient,
        //            radius = scorchExplosionRadius,
        //            baseForce = 10f,
        //            crit = damageReport.attackerBody.RollCrit(),
        //            position = victimBody.corePosition,
        //            inflictor = damageReport.attacker,
        //            attacker = damageReport.attacker,
        //            teamIndex = damageReport.attackerBody.teamComponent.teamIndex,
        //            damageType = DamageType.Stun1s,
        //            damageColorIndex = DamageColorIndex.Default
        //        };
        //        DamageAPI.AddModdedDamageType(blastAttack, Quo_GinPlugin.ScorchMark);
        //        victimBody.RemoveBuff(Modules.Buffs.scorchDebuff);
        //        blastAttack.Fire();
        //    }
        //}


        public void Hook_ScorchDebuff()
        {
            Log.Debug("hooking Scorch Debuff");
           // On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            //On.RoR2.GlobalEventManager.OnHitAll += GlobalEventManager_OnHitAll;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            //On.RoR2.GlobalEventManager.ServerDamageDealt += GlobalEventManager_ServerDamageDealt;
            //On.RoR2.GlobalEventManager.ServerDamageDealt += GlobalEventManager_ServerDamageDealt;
        }


        //private void GlobalEventManager_ServerDamageDealt(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig,GlobalEventManager self, DamageReport damageReport )
        //{
        //    orig(self, damageReport);

        //    OnTakeDamageServer(damageReport);
        //}
        private void GlobalEventManager_ServerDamageDealt(On.RoR2.GlobalEventManager.orig_ServerDamageDealt orig, DamageReport damageReport)
        {
            orig(damageReport);
            Log.Debug("Damage Dealt");

            CharacterBody victimBody = damageReport.victimBody;

            EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
            {
                origin = victimBody.corePosition,
                scale = scorchExplosionRadius,
            }, true);
            BlastAttack blastAttack = new BlastAttack()
            {
                baseDamage = damageReport.damageDealt * scorchExplosionCoefficient,
                radius = scorchExplosionRadius,
                baseForce = 10f,
                crit = damageReport.damageInfo.crit,
                position = victimBody.corePosition,
                inflictor = damageReport.attacker,
                attacker = damageReport.attacker,
                teamIndex = damageReport.attacker.GetComponent<TeamIndex>(),
                damageType = DamageType.Stun1s,
                damageColorIndex = DamageColorIndex.Default,

            };
            DamageAPI.AddModdedDamageType(blastAttack, Quo_GinPlugin.ScorchMark);
            victimBody.ClearTimedBuffs(Modules.Buffs.scorchDebuff.buffIndex);
            blastAttack.Fire();
            victimBody.RemoveBuff(Modules.Buffs.scorchDebuff);

        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            
            damageInfo.damage *= 1f + (self.body.GetBuffCount(Modules.Buffs.scorchDebuff) * damagePerStack);

            if (DamageAPI.HasModdedDamageType(damageInfo, Quo_GinPlugin.ScorchMark))
            {

                Log.Debug("Adding Scorch Buff");
                self.body.AddTimedBuff(Modules.Buffs.scorchDebuff, scorchDuration);
            }
            orig(self, damageInfo);
            if (self.body.GetBuffCount(Modules.Buffs.scorchDebuff) > scorchStacksToExplode)
            {
                self.body.ClearTimedBuffs(Modules.Buffs.scorchDebuff.buffIndex);
                Log.Debug("Scorch Explosion");
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
                {
                    origin = self.body.corePosition,
                    scale = scorchExplosionRadius,
                }, true);
                BlastAttack blastAttack = new BlastAttack()
                {
                    baseDamage = damageInfo.damage * scorchExplosionCoefficient,
                    radius = scorchExplosionRadius,
                    baseForce = 10f,
                    crit = damageInfo.crit,
                    position = self.body.corePosition,
                    inflictor = damageInfo.inflictor,
                    attacker = damageInfo.attacker,
                    teamIndex = damageInfo.attacker.GetComponent<TeamIndex>(),
                    damageType = DamageType.Stun1s,
                    damageColorIndex = DamageColorIndex.Default,
                    attackerFiltering = AttackerFiltering.NeverHitSelf 
                };
                DamageAPI.AddModdedDamageType(blastAttack, Quo_GinPlugin.ScorchMark);
                blastAttack.Fire();
                self.body.RemoveBuff(Modules.Buffs.scorchDebuff);
            }

        }
        private void GlobalEventManager_OnHitAll(On.RoR2.GlobalEventManager.orig_OnHitAll orig, GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
        {
            CharacterBody victimBody = hitObject.GetComponent<CharacterBody>();
            damageInfo.damage *= 1f + (victimBody.GetBuffCount(Modules.Buffs.scorchDebuff) * damagePerStack);

            if (DamageAPI.HasModdedDamageType(damageInfo, Quo_GinPlugin.ScorchMark))
            {

                Log.Debug("Adding Scorch Buff");
                victimBody.AddTimedBuff(Modules.Buffs.scorchDebuff, scorchDuration);
            }
            
            orig(self, damageInfo, hitObject);

            if (victimBody.GetBuffCount(Modules.Buffs.scorchDebuff) > scorchStacksToExplode)
            {
                victimBody.ClearTimedBuffs(Modules.Buffs.scorchDebuff.buffIndex);
                Log.Debug("Scorch Explosion");
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
                {
                    origin = victimBody.corePosition,
                    scale = scorchExplosionRadius,
                }, true);
                BlastAttack blastAttack = new BlastAttack()
                {
                    baseDamage = damageInfo.damage * scorchExplosionCoefficient,
                    radius = scorchExplosionRadius,
                    baseForce = 10f,
                    crit = damageInfo.crit,
                    position = victimBody.corePosition,
                    inflictor = damageInfo.attacker,
                    attacker = damageInfo.attacker,
                    teamIndex = damageInfo.attacker.GetComponent<TeamIndex>(),
                    damageType = DamageType.Stun1s,
                    damageColorIndex = DamageColorIndex.Default,

                };
                //DamageAPI.AddModdedDamageType(blastAttack, Quo_GinPlugin.ScorchMark);
                //victimBody.RemoveBuff(Modules.Buffs.scorchDebuff);
                blastAttack.Fire();

            }

        }

    }
}
