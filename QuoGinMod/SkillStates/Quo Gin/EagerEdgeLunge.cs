using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using R2API;
using UnityEngine;
using EntityStates;
using System.Linq;



namespace Quo_Gin.SkillStates 
{
    internal class EagerEdgeLunge : BaseSkillState
    {
        public static float damageCoefficient = 7f;
        private Vector3 lungeVector;
        private Transform modelTransform;
        private float duration = 1f;
        private float stopWatch = 0f;
        private float speedCoefficient = 2f;

        private HitBox hitBox;
        private bool hasFired = false;
        private OverlapAttack attack;
        private Vector3 lungeVelocity
        {
            get
            {
                return this.lungeVector * this.moveSpeedStat *speedCoefficient;
            }
        }
        public override void OnEnter()
        {
            base.OnEnter();
            Log.Debug("Entering Lunge");
            this.lungeVector = base.inputBank.aimDirection;
            
            HitBoxGroup  hitboxGroup = Array.Find<HitBoxGroup>(base.GetModelTransform().GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "EagerEdge");
            base.gameObject.layer = LayerIndex.fakeActor.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = Vector3.zero;
            
            this.modelTransform = base.GetModelTransform();
            base.characterDirection.forward = base.characterMotor.velocity.normalized;


            Ray aimRay = base.GetAimRay();
            this.attack = new OverlapAttack()
            {
                damageType = DamageType.Stun1s,
                attacker = base.gameObject,
                inflictor = base.gameObject,
                teamIndex = base.GetTeam(),
                damage = base.damageStat * damageCoefficient,
                procCoefficient = 1f,
                forceVector = aimRay.direction * 3f,
                pushAwayForce = 100f,
                hitBoxGroup = hitboxGroup,
                isCrit=base.RollCrit()
            };
            Log.Debug(hitboxGroup.hitBoxes[0].transform.localScale);

        }
        public override void OnExit()
        {
            //Log.Debug("ON Exit Lunge");
            base.SmallHop(base.characterMotor, -3f);
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopWatch += Time.fixedDeltaTime;
            if (this.stopWatch >= this.duration)
            {
                Log.Debug("Exiting Lunge Late");
                if (!hasFired)
                {
                    this.hasFired = true;
                    this.attack.damage *= 1 + 1 / base.moveSpeedStat;
                    this.attack.Fire();
                }
                this.outer.SetNextStateToMain();
            }

            else if (!hasFired)
            {
                base.characterMotor.rootMotion += this.lungeVelocity * Time.fixedDeltaTime;
                base.characterDirection.forward = this.lungeVelocity;
                base.characterDirection.moveVector = this.lungeVelocity;
                base.characterBody.isSprinting = true;

                if (earlySwing(5f))
                {
                    Log.Debug("Swinging Early");
                    this.hasFired = true;
                    this.attack.damage *= 1 + 1 / 2 * base.moveSpeedStat;
                    this.attack.Fire();
                }
            }
            else
            {
                Log.Debug("Exiting Lunge Early");
                this.outer.SetNextStateToMain();
            }
            
        }

        private bool earlySwing(float radius)
        {
            Ray aimRay = base.GetAimRay();
            BullseyeSearch bullseyeSearch = new BullseyeSearch()
            {
                teamMaskFilter = TeamMask.GetEnemyTeams(base.GetTeam()),
                filterByLoS = false,
                searchOrigin = base.transform.position,
                searchDirection = UnityEngine.Random.onUnitSphere,
                sortMode = BullseyeSearch.SortMode.Distance,
                maxAngleFilter = 90f,
                maxDistanceFilter = radius
            };
            bullseyeSearch.RefreshCandidates();
            bullseyeSearch.FilterOutGameObject(base.gameObject);
            HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
            if (hurtBox && hurtBox.healthComponent && hurtBox.healthComponent.body)
            {
                return true;
            }
            return false;
        }

        //public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
        //{
        //    base.AuthorityModifyOverlapAttack(overlapAttack);
        //    overlapAttack.damage = this.damageStat * damageCoefficient;
        //}
        //public override void OnMeleeHitAuthority()
        //{
        //    base.OnMeleeHitAuthority();
        //    float num = this.hitPauseDuration / this.attackSpeedStat;
            
        //    foreach (HurtBox victimHurtBox in this.hitResults)
        //    {
        //        float damageValue = base.characterBody.damage * damageCoefficient;
        //        bool isCrit = base.RollCrit();
        //    }
        //}
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
