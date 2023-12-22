using EntityStates;
using Quo_Gin.SkillStates;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using  RoR2.Projectile;

namespace Quo_Gin.SkillStates
{
    internal class CelestialFire : BaseState
    {

        public int projectileCount = 3;
        public static int minProjectileCount = 3;
        public static int maxProjectileCount = 7;
        public static float damageCoefficient = 2f;
        public static float damageRadius = 1f;
        public static float projectileSpeed = 80f;

        private GameObject projectilePrefab = Modules.Projectiles.celestialFirePrefab;


        private static float baseDuration = 2f;
        private static float baseFireDuration = .4f;
        private float duration = 2f;
        private float fireDuration = 1f;
        private int projectilesFired = 0;

        private float totalYawSpread = 1f;
        private float totalPitchSpread = 1f;
        private float force = 10f;
        
        public override void OnEnter()
        {
            base.OnEnter();
            Log.Debug($"Base Projectile: {projectileCount}");
           // projectileCount *= Mathf.FloorToInt( 1f * this.attackSpeedStat);
           // Log.Debug($"Current Projectile: {projectileCount}");
            this.duration = baseDuration/ this.attackSpeedStat;
            this.fireDuration= baseFireDuration/ this.attackSpeedStat;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            int num = Mathf.FloorToInt(base.fixedAge/this.fireDuration * projectileCount);
            if (this.projectilesFired <= num && this.projectilesFired < this.projectileCount)
            {
                this.PlayAnimation("Gesture, Additive", "FireSyringe");
                base.characterBody.SetAimTimer(3f);
                if (base.isAuthority)
                {
                    Ray aimRay = base.GetAimRay();
                    float bonusYaw = (float)Mathf.FloorToInt((float)this.projectilesFired - (float)(projectileCount - 1) / 2f) / (float)(projectileCount - 1) * totalYawSpread;
                    Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw, 0f);
                    ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), 
                        base.gameObject, this.damageStat * damageCoefficient, force, Util.CheckRoll(this.critStat, base.characterBody.master), 
                        DamageColorIndex.Default, null, -1f);
                }
                this.projectilesFired++;
                Log.Debug($"Celestial Fired{this.projectilesFired}");
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
