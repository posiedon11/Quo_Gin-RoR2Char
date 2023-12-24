using BepInEx;
using Quo_Gin.Componenets;
using Quo_Gin.Modules.Survivors;
using Quo_Gin.SkillStates;
using R2API;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace  Quo_Gin
{

    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "UnlockableAPI"
    })]

    public class Quo_GinPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.posiedon11.Quo-Gin";
        public const string MODNAME = "Quo-Gin";
        public const string MODVERSION = "1.0.0";
        

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "POSIEDON11";

        public static Quo_GinPlugin instance;



        public static DamageAPI.ModdedDamageType SunShotMark;
        public static DamageAPI.ModdedDamageType ScorchMark;
        private void Awake()
        {
            instance = this;

            Quo_GinPlugin.SunShotMark = DamageAPI.ReserveDamageType();
            Quo_GinPlugin.ScorchMark = DamageAPI.ReserveDamageType();

            Log.Init(Logger);
            Modules.Tokens.AddTokens();
            Modules.Assets.Initialize(); // load assets and read config
            Modules.Config.ReadConfig();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
             // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            

            // survivor initialization
            new Quo_GinChar().Initialize();
            //ScorchDebuff scorchController = new ScorchDebuff();
            //scorchController.Start();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();
            

            Hook();
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            //Hook_SunShot();
            new ScorchDebuff().Hook_ScorchDebuff();
            new SunShotMark().Hook_SunShot();
            new AscendingDawnBuff().Hook_AscendingDawn();
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            // a simple stat hook, adds armor after stats are recalculated
            if (self)
            {
                if (self.HasBuff(Modules.Buffs.armorBuff))
                {
                    self.armor += 300f;
                }

                if (self.HasBuff(Modules.Buffs.wellOfRadianceBuff))
                {
                    self.damage *= 1+WellOfRadiance.damageBuffBonusDamage;
                }

                if(self.HasBuff(Modules.Buffs.ascendingDawnKillBuff))
                {
                    self.moveSpeed *= 1 + AscendingDawn.killSpeedCoefficient;
                }
            }
        }


        private void Hook_SunShot()
        {
            //On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            //On.RoR2.GlobalEventManager.OnHitAll += GlobalEventManager_OnHitAll;
        }

        private void GlobalEventManager_OnHitAll(On.RoR2.GlobalEventManager.orig_OnHitAll orig,GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
        {
            orig(self, damageInfo, hitObject);
            bool hasSunShotMark = DamageAPI.HasModdedDamageType(damageInfo, Quo_GinPlugin.SunShotMark);
            {
                float sunShotExplosion = (5) * damageInfo.procCoefficient;
                float sunShotdamageCoefficient = 0.6f;
                float sunShotbaseDamage = Util.OnHitProcDamage(damageInfo.damage,damageInfo.attacker.GetComponent<CharacterBody>().damage, sunShotdamageCoefficient);
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
                {
                    origin = damageInfo.position,
                    scale = sunShotExplosion,
                    rotation = Util.QuaternionSafeLookRotation(damageInfo.force)
                }, true);
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.position = damageInfo.position;
                blastAttack.baseDamage = sunShotbaseDamage;
                blastAttack.baseForce = 0f;
                blastAttack.radius = sunShotExplosion;
                blastAttack.attacker = damageInfo.attacker;
                blastAttack.inflictor = null;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.crit = damageInfo.crit;
                blastAttack.procChainMask = damageInfo.procChainMask;
                blastAttack.procCoefficient = 0f;
                blastAttack.damageColorIndex = DamageColorIndex.Item;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.damageType = damageInfo.damageType;
                blastAttack.Fire();
            }

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

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);

            if (damageReport.victimBody)
            {
                if (damageReport.victimBody.HasBuff(Quo_Gin.Modules.Buffs.sunShotDebuff))
                {
                    //GlobalEventManager.ProcIgniteOnKill(damageReport, 5, damageReport.victimBody, damageReport.attackerTeamIndex);
                }
            }
        }
    }
}