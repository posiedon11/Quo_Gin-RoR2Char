using EntityStates;
using Quo_Gin.SkillStates;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using static RoR2.BulletAttack;
namespace Quo_Gin.SkillStates
{
    internal class SunShot :GenericProjectileBaseState
    {
        public static float damageCoefficient = 2f;
        public static float baseDuration = 0.25f;
        public static float force = 800f;


        public static float procCoefficient = 1f; 
        public static float recoil = 6f;
        public static float range = 256f;
        public static float explosionDamageCoefficient = .5f;
        public static float explosionRadius = 10f;
        public static GameObject tracerEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGoldGat");



        new private float duration;
        private float fireTime;
        private bool hasFired;
        private string muzzleString;

        private bool hasExploded = false;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = SunShot.baseDuration / this.attackSpeedStat;
            this.fireTime = 0.2f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.muzzleString = "Muzzle";
            this.hasFired = false;

            base.PlayAnimation("LeftArm, Override", "ShootGun", "ShootGun.playbackRate", 1.8f);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void Fire()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;

                base.characterBody.AddSpreadBloom(1.5f);
               // EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);
               // Util.PlaySound("HenryShootPistol", base.gameObject);

                if (base.isAuthority)
                {
                    Ray aimRay = base.GetAimRay();
                    base.AddRecoil(-1f * SunShot.recoil, -2f * SunShot.recoil, -0.5f * SunShot.recoil, 0.5f * SunShot.recoil);

                    BulletAttack bulletAttack = new BulletAttack
                    {
                        owner = base.gameObject,
                        weapon = base.gameObject,
                        origin = aimRay.origin,
                        aimVector = aimRay.direction,

                        bulletCount = 1u,
                        damage = SunShot.damageCoefficient * this.damageStat,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Generic,
                        falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                        maxDistance = SunShot.range,
                        force = SunShot.force,
                        hitMask = LayerIndex.CommonMasks.bullet,
                        minSpread = 0f,
                        maxSpread = 0f,
                        isCrit = base.RollCrit(),
                        
                        muzzleName = muzzleString,
                        smartCollision = false,
                        procChainMask = default(ProcChainMask),
                        procCoefficient = procCoefficient,
                        radius = 0.75f,
                        sniper = false,
                        stopperMask = LayerIndex.CommonMasks.bullet,
                        
                        tracerEffectPrefab = SunShot.tracerEffectPrefab,
                        hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                        hitCallback = ExplosionCallBack
      
                    };
                    DamageAPI.AddModdedDamageType(bulletAttack, Quo_GinPlugin.SunShotMark);
                    bulletAttack.Fire();       
                }
            }
        }

        private bool ExplosionCallBack(BulletAttack bulletRef, ref BulletHit hitInfo)
        {
            if (hitInfo.point != null && !hasExploded)
            {
                hasExploded = true;

                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
                {
                    origin = hitInfo.point,
                    scale = explosionRadius,
                }, true);
                BlastAttack blastAttack = new BlastAttack
                {
                    attacker = base.gameObject,
                    teamIndex = base.characterBody.teamComponent.teamIndex,
                    baseDamage = damageCoefficient * this.damageStat * explosionDamageCoefficient,
                    baseForce = 2f,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    crit = bulletRef.isCrit,
                    damageColorIndex = DamageColorIndex.Item,
                    damageType = DamageType.Generic,
                    falloffModel = BlastAttack.FalloffModel.None,
                    inflictor = base.gameObject,
                    position = hitInfo.point,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = 1f,
                    radius = explosionRadius,
                    
                };
                DamageAPI.AddModdedDamageType(blastAttack, Quo_GinPlugin.ScorchMark);
                blastAttack.Fire();
            }
             return BulletAttack.defaultHitCallback.Invoke(bulletRef, ref hitInfo);

        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireTime)
            {
                this.Fire();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }

}
