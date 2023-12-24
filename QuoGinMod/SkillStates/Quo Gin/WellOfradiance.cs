using EntityStates;
using Quo_Gin.SkillStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using EntityStates.Huntress;
using EntityStates.Croco;
using EntityStates.Mage;
using Quo_Gin.Modules;
using RoR2.Projectile;
using R2API;

namespace Quo_Gin.SkillStates
{
    internal class WellOfRadiance : BaseSkillState
    {
        public static float landingDamageCoefficient = 20f;
        public static float damageBuffBonusDamage = .5f;
        public static float landingRadius = 40f;
        public static float landingForce = 1000f;
        public static float healingCoefficient = .5f;
        public static float healingPulseBaseFrequency = 2f;
        public static float healingPulseSpeedStat = 1f;
        public static float wellDuration = 30f;
        public static float wellBuffduration = 2f;

        private GameObject wellPrefab;
        private bool hasDroppped = false;
        private static float dropForce = 150f;
        private Ray downRay;
        private Transform landingIndicator;
        private static float stallDuration = .5f;



        public override void OnEnter()
        {
            base.OnEnter();
            this.wellPrefab = Projectiles.wellOfRadiancePrefab;
            Log.Debug("Entering Well of Radiance");
            healingPulseSpeedStat = base.attackSpeedStat;
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = Vector3.zero;
            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            base.characterMotor.Motor.RebuildCollidableLayers();
            //base.characterMotor.rootMotion = Vector3.up * .2f;
            //base.characterMotor.rootMotion += Vector3.up * (0.2f * this.moveSpeedStat * FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / stallDuration) * Time.fixedDeltaTime);
            base.SmallHop(characterMotor, 3);
        }

        public override void Update()
        {
            base.Update();
            if (this.landingIndicator)
            {
                this.updateLandingIndicator();
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!this.hasDroppped)
            {
                //if(base.fixedAge < .2f)
                //{
                    //base.characterMotor.rootMotion += Vector3.up * (0.2f * this.moveSpeedStat * FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / stallDuration) * Time.fixedDeltaTime);
              //  }
                 if (base.fixedAge < stallDuration)
                {
                    if (base.characterMotor.velocity.y < .2f)
                    {
                        Log.Debug("Stalling");
                        base.characterMotor.velocity.y += .2f;
                    }
                }
               //base.characterMotor.rootMotion += Vector3.up * (0.2f * this.moveSpeedStat * FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / stallDuration) * Time.fixedDeltaTime);
                
            }

            if (base.fixedAge >= .2f * stallDuration && this.landingIndicator)
            {
                this.createIndicator();
            }
            if (base.fixedAge >= stallDuration && !this.hasDroppped)
            {
                this.startDrop();
            }
            if (this.hasDroppped && base.isAuthority && !base.characterMotor.disableAirControlUntilCollision)
            {
                this.landingImpact();
                this.outer.SetNextStateToMain();
            }

        }
        public override void OnExit()
        {
            base.OnExit();

            if (this.landingIndicator) 
            {
                EntityState.Destroy(this.landingIndicator);
            }
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;

            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
        }

        private void createWell()
        {

            if (    base.isAuthority && this.wellPrefab)
            {
                Log.Debug("Creating Well");
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo()
                {
                    projectilePrefab = this.wellPrefab,
                    position = base.characterBody.footPosition,
                    owner = base.gameObject,
                    damage = this.damageStat,
                    crit = false
                };

                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            }
        }
        private void landingImpact()
        {
            Log.Debug("Landing");
            createWell();
            base.characterMotor.velocity = Vector3.one;
            BlastAttack blastAttack = new BlastAttack()
            {
                radius = landingRadius,
                procCoefficient = 1f,
                position = base.characterBody.footPosition,
                attacker = base.gameObject,
                crit = base.RollCrit(),
                baseDamage = base.characterBody.damage * landingDamageCoefficient,
                falloffModel = BlastAttack.FalloffModel.None,
                baseForce = landingForce,
                teamIndex = base.gameObject.GetComponent<TeamIndex>(),
                damageType = DamageType.IgniteOnHit,
                attackerFiltering = AttackerFiltering.NeverHitSelf
            };
            EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
            {
                origin = base.characterBody.footPosition,
                scale = landingRadius,
                rotation = Util.QuaternionSafeLookRotation(Vector3.zero)
            }, true);
            DamageAPI.AddModdedDamageType(blastAttack, Quo_GinPlugin.ScorchMark);
            blastAttack.Fire();
            
            
        }
        private void startDrop()
        {
            Log.Debug("Dropping");
            this.hasDroppped = true;
            base.characterMotor.disableAirControlUntilCollision = true;
            if (!base.characterMotor.isGrounded)
            {
                Log.Debug("Char isnt grounded");
                base.characterMotor.velocity.y = -dropForce;
            }
            else
                Log.Debug("Char isnt grounded");
        }

        private void createIndicator()
        {
            if ( ArrowRain.areaIndicatorPrefab)
            {
                this.downRay = new Ray()
                {
                    direction = Vector3.down,
                    origin = base.transform.position
                };

                this.landingIndicator = UnityEngine.Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab).transform;
                this.landingIndicator.localScale = Vector3.one * landingRadius;
            }
        }
        private void updateLandingIndicator()
        {
            if (this.landingIndicator)
            {
                float maxdistance = 250f;
                this.downRay = new Ray()
                {
                    direction = Vector3.down,
                    origin = base.transform.position
                };
                RaycastHit raycastHit;
                if(Physics.Raycast(this.downRay, out raycastHit, maxdistance, LayerIndex.world.mask))
                {
                    this.landingIndicator.transform.position = raycastHit.point;
                    this.landingIndicator.transform.up = raycastHit.normal;
                }
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
