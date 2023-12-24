using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using R2API;
using UnityEngine;
using EntityStates;



namespace Quo_Gin.SkillStates 
{
    internal class EagerEdgeLunge : BasicMeleeAttack
    {
        public static float damageCoefficient = 7f;
        private Vector3 lungeVector;
        private Transform modelTransform;
        private float duration = 2f;


        public string enterAnimationLayerName = "FullBody, Override";

        // Token: 0x04000CEC RID: 3308
        [SerializeField]
        public string enterAnimationStateName = "AssaulterLoop";

        // Token: 0x04000CED RID: 3309
        [SerializeField]
        public float enterAnimationCrossfadeDuration = 0.1f;

        // Token: 0x04000CEE RID: 3310
        [SerializeField]
        public string exitAnimationLayerName = "FullBody, Override";

        // Token: 0x04000CEF RID: 3311
        [SerializeField]
        public string exitAnimationStateName = "EvisLoopExit";
        private Vector3 lungeVelocity
        {
            get
            {
                return this.lungeVector * this.moveSpeedStat;
            }
        }
        public override void OnEnter()
        {
            base.OnEnter();
            Log.Debug("Entering Lunge");
            this.lungeVector = base.inputBank.aimDirection;


            base.gameObject.layer = LayerIndex.fakeActor.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = Vector3.zero;

            this.modelTransform = base.GetModelTransform();
            base.PlayCrossfade(this.enterAnimationLayerName, this.enterAnimationStateName, this.enterAnimationCrossfadeDuration);
            base.characterDirection.forward = base.characterMotor.velocity.normalized;
        }
        public override void OnExit()
        {
            Log.Debug("Exiting Lunge");
            base.SmallHop(base.characterMotor, -10f);
            this.PlayAnimation(this.exitAnimationLayerName, this.exitAnimationStateName);
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            base.OnExit();
        }
        public override void PlayAnimation()
        {
            base.PlayAnimation();
            // base.PlayCrossfade(this.enterAnimationLayerName, this.enterAnimationStateName, this.enterAnimationCrossfadeDuration);
            base.PlayCrossfade("Gesture, Override", "Slash" + (1), "Slash.playbackRate", this.duration, 0.05f);

        }
        public override void AuthorityFixedUpdate()
        {
            base.AuthorityFixedUpdate();
            if (!base.authorityInHitPause)
            {
                base.characterMotor.rootMotion += this.lungeVelocity * Time.fixedDeltaTime;
                base.characterDirection.forward = this.lungeVelocity;
                base.characterDirection.moveVector = this.lungeVelocity;
                base.characterBody.isSprinting = true;
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this.stopwatch)
        }

        public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
        {
            base.AuthorityModifyOverlapAttack(overlapAttack);
            overlapAttack.damage = this.damageStat * damageCoefficient;
        }
        public override void OnMeleeHitAuthority()
        {
            base.OnMeleeHitAuthority();
            float num = this.hitPauseDuration / this.attackSpeedStat;
            
            foreach (HurtBox victimHurtBox in this.hitResults)
            {
                float damageValue = base.characterBody.damage * damageCoefficient;
                bool isCrit = base.RollCrit();
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
