using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using EntityStates;
using RoR2;
using HG;
using UnityEngine.UIElements;
using R2API;

namespace Quo_Gin.SkillStates
{
    internal class AscendingDawn : BaseSkillState
    {
        public static float jumpDuration = 3f;
        public static float verticalJumpPower = 50f;
        public static float fowardJumpPower = 20f;


        public static string dodgeSoundString = "HenryRoll";
        public static float dodgeFOV = EntityStates.Commando.DodgeState.dodgeFOV;


        public static float damageCoefficient = 6f;
        public static float damageRadius = 25f;
        public static float killSpeedCoefficient = 3f;
        public static float killSpeedDuration = 6f;
        public static float jumpBuffDuration = 5f;

        private static float jumpStopWatch;


        private float previousAircontrol;
        private Vector3 forwardDirection;
        private float dampingStrength;
        private float dampingVelocity;
        private float minDamping = .5f;
        private float maxDamping = .1f;
        private Animator animator;
        private BlastAttack blastAttack;

        public override void OnEnter()
        {
            base.OnEnter();
            Log.Debug("entering Ascending Dawn");
            this.animator = base.GetModelAnimator();

            this.skillLocator.secondary.Reset();
           // this.skillLocator.secondary.AddOneStock();
            this.blastAttack = new BlastAttack()
            {

                baseDamage = base.damageStat * damageCoefficient,
                baseForce = 20f,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                crit = false,
                damageColorIndex = DamageColorIndex.Item,
                damageType = DamageType.Generic,
                falloffModel = BlastAttack.FalloffModel.None,
                inflictor = base.gameObject,
                position = base.characterBody.footPosition,
                procChainMask = default(ProcChainMask),
                procCoefficient = 1f,
                radius = damageRadius,
                teamIndex = base.teamComponent.teamIndex
            };
            EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
            {
                origin = base.characterBody.footPosition,
                scale = damageRadius,
                rotation = Util.QuaternionSafeLookRotation(Vector3.zero)
            }, true);
            Log.Debug("Creating Explosion For dawn");
            DamageAPI.AddModdedDamageType(this.blastAttack, Quo_GinPlugin.ScorchMark);
            this.blastAttack.Fire();
            if (NetworkServer.active)
            {
                base.characterBody.AddTimedBuff(Modules.Buffs.ascendingDawnJumpBuff, jumpBuffDuration);
            }




            if (base.isAuthority && base.inputBank && base.characterDirection)
            {
                this.forwardDirection = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            }

            base.characterMotor.disableAirControlUntilCollision = false;
            //this.previousAircontrol = base.characterMotor.airControl;
          //  base.characterMotor.airControl = 1f;
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = (Vector3.up * verticalJumpPower) + (this.forwardDirection * fowardJumpPower);
 

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge>= jumpDuration)
            {
                jumpStopWatch += Time.fixedDeltaTime;

                this.dampingStrength = Mathf.SmoothDamp(this.dampingStrength, maxDamping, ref this.dampingVelocity, jumpDuration);
            }

      
                this.outer.SetNextStateToMain();
            
        }

        public override void OnExit()
        {
           // base.characterMotor.airControl = this.previousAircontrol;
            base.OnExit();

        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.forwardDirection);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.forwardDirection = reader.ReadVector3();
        }
    }
}
