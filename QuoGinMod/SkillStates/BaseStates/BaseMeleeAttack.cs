using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Quo_Gin.SkillStates.BaseStates
{
    public class BaseMeleeAttack : BaseSkillState
    {
        public int swingIndex;

        protected string hitboxName = "Sword";

        protected DamageType damageType = DamageType.Generic;
        protected float damageCoefficient = 3.5f;
        protected float procCoefficient = 1f;
        protected float pushForce = 300f;
        protected Vector3 bonusForce = Vector3.zero;
        protected float baseDuration = 1f;
        protected float attackStartTime = 0.2f;
        protected float attackEndTime = 0.4f;
        protected float baseEarlyExitTime = 0.4f;
        protected float hitStopDuration = 0.012f;
        protected float attackRecoil = 0.75f;
        protected float hitHopVelocity = 4f;
        protected bool cancelled = false;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString = "SwingCenter";
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab;
        protected NetworkSoundEventIndex impactSound;

        private float earlyExitTime;
        public float duration;
        private bool hasFired;
        private float hitPauseTimer;
        private OverlapAttack attack;
        protected bool inHitPause;
        private bool hasHopped;
        protected float stopwatch;
        protected Animator animator;
        private HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            earlyExitTime = baseEarlyExitTime / attackSpeedStat;
            hasFired = false;
            animator = GetModelAnimator();
            StartAimMode(0.5f + duration, false);
            characterBody.outOfCombatStopwatch = 0f;
            animator.SetBool("attacking", true);

            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (element) => element.groupName == hitboxName);
            }

            PlayAttackAnimation();

            attack = new OverlapAttack();
            attack.damageType = damageType;
            attack.attacker = gameObject;
            attack.inflictor = gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = damageCoefficient * damageStat;
            attack.procCoefficient = procCoefficient;
            attack.hitEffectPrefab = hitEffectPrefab;
            attack.forceVector = bonusForce;
            attack.pushAwayForce = pushForce;
            attack.hitBoxGroup = hitBoxGroup;
            attack.isCrit = RollCrit();
            attack.impactSound = impactSound;
        }

        protected virtual void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash" + (1 + swingIndex), "Slash.playbackRate", duration, 0.05f);
        }

        public override void OnExit()
        {
            if (!hasFired && !cancelled) FireAttack();

            base.OnExit();

            animator.SetBool("attacking", false);
        }

        protected virtual void PlaySwingEffect()
        {
            EffectManager.SimpleMuzzleFlash(swingEffectPrefab, gameObject, muzzleString, true);
        }

        protected virtual void OnHitEnemyAuthority()
        {
            Util.PlaySound(hitSoundString, gameObject);

            if (!hasHopped)
            {
                if (characterMotor && !characterMotor.isGrounded && hitHopVelocity > 0f)
                {
                    SmallHop(characterMotor, hitHopVelocity);
                }

                hasHopped = true;
            }

            if (!inHitPause && hitStopDuration > 0f)
            {
                storedVelocity = characterMotor.velocity;
                hitStopCachedState = CreateHitStopCachedState(characterMotor, animator, "Slash.playbackRate");
                hitPauseTimer = hitStopDuration / attackSpeedStat;
                inHitPause = true;
            }
        }

        private void FireAttack()
        {
            if (!hasFired)
            {
                hasFired = true;
                Util.PlayAttackSpeedSound(swingSoundString, gameObject, attackSpeedStat);

                if (isAuthority)
                {
                    PlaySwingEffect();
                    AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
                }
            }

            if (isAuthority)
            {
                if (attack.Fire())
                {
                    OnHitEnemyAuthority();
                }
            }
        }

        protected virtual void SetNextState()
        {
            int index = swingIndex;
            if (index == 0) index = 1;
            else index = 0;

            outer.SetNextState(new BaseMeleeAttack
            {
                swingIndex = index
            });
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            hitPauseTimer -= Time.fixedDeltaTime;

            if (hitPauseTimer <= 0f && inHitPause)
            {
                ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
                inHitPause = false;
                characterMotor.velocity = storedVelocity;
            }

            if (!inHitPause)
            {
                stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if (characterMotor) characterMotor.velocity = Vector3.zero;
                if (animator) animator.SetFloat("Swing.playbackRate", 0f);
            }

            if (stopwatch >= duration * attackStartTime && stopwatch <= duration * attackEndTime)
            {
                FireAttack();
            }

            if (stopwatch >= duration - earlyExitTime && isAuthority)
            {
                if (inputBank.skill1.down)
                {
                    if (!hasFired) FireAttack();
                    SetNextState();
                    return;
                }
            }

            if (stopwatch >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(swingIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            swingIndex = reader.ReadInt32();
        }
    }
}