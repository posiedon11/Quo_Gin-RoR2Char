using EntityStates;
using Quo_Gin.Componenets;
using Quo_Gin.Modules;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Quo_Gin.SkillStates
{
    internal class SolarGrenade: AimThrowableBase
    {
       // public static float BaseDuration = 0.35f;
        //delay here for example and to match animation
        //ordinarily I recommend not having a delay before projectiles. makes the move feel sluggish
       // public static float BaseDelayDuration = 0.35f * BaseDuration;

       
        public static float solarGrenadeDamageCoefficient = 5f;
        public static float damageRadius = 6f;
        public static float duration = 12f;
        public static float basePulseSpeed = 3f;
        public static float grenadeaAttackSpeedStat = 3f;
        public static float activeGrenades = 0f;

        public override void OnEnter()
        {

            Log.Message("Throwing Projectile");
            this.projectilePrefab = Modules.Projectiles.solarGrenadePrefab;
            this.maxDistance = 150f;
            this.rayRadius = 2f;
            this.arcVisualizerPrefab = Assets.lineVisualizer;
            this.endpointVisualizerPrefab = Assets.explosionvisualizer;
            this.endpointVisualizerRadiusScale = damageRadius;
            this.detonationRadius = damageRadius;
            this.damageCoefficient = solarGrenadeDamageCoefficient;
            this.setFuse = false;
            this.projectileBaseSpeed = 60f;
            this.minimumDuration = 0f;
            grenadeaAttackSpeedStat = base.attackSpeedStat;
            
            base.PlayAnimation("Gesture, Override", "BufferEmpty");
            base.PlayAnimation("Grenade, Override", "AimGrenade");
            base.OnEnter();

        }

        public override void FixedUpdate()
        {
            base.StartAimMode(0f, false);
            base.FixedUpdate(); 
        }
        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("Grenade, Override", "ThrowGrenade");
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
