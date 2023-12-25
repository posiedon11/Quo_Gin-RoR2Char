
using EntityStates;
using Quo_Gin.Modules;
using Quo_Gin.Modules.Characters;
using Quo_Gin.SkillStates;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


namespace Quo_Gin.Componenets
{
    internal class SolarGrenadePulseController : MonoBehaviour
    {
        private GameObject owner;
        private SphereSearch search;
        private List<HurtBox> candidates;
        private TeamFilter teamFilter;
 
 

        private float pulseInterval = 3f;
        private float pulseStopWatch = 0f;
        private float radius = SolarGrenade.damageRadius;

        private Vector3 position;
        private BlastAttack blastAttack;

        private void Awake()
        {
            this.teamFilter = base.GetComponent<TeamFilter>();
            this.search = new SphereSearch();
            this.position = base.transform.position;
            this.candidates= new List<HurtBox>();
            this.pulseInterval = SolarGrenade.basePulseSpeed/ SolarGrenade.grenadeaAttackSpeedStat;
            Log.Message("Pulse Awake");

        }
        private void Start()
        {
            Log.Message("Grenade Blew Up");
            this.owner = base.GetComponent<ProjectileController>().owner;
            this.blastAttack = new BlastAttack()
            {
                baseDamage = base.GetComponent<ProjectileDamage>().damage,
                baseForce = 2f,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                crit = false,
                damageColorIndex = DamageColorIndex.Item,
                damageType = DamageType.Generic,
                falloffModel = BlastAttack.FalloffModel.None,
                inflictor = base.gameObject,
                position = position,
                procChainMask = default(ProcChainMask),
                procCoefficient = 1f,
                radius = radius,
                teamIndex = teamFilter.teamIndex
            };    
            pulseInterval /=  .25f *owner.GetComponent<CharacterBody>().attackSpeed;
        }

        private void FixedUpdate()
        {
            //Log.Message("Entering Pulse");
            if (NetworkServer.active)
            {
                //Log.Message($"{this.pulseStopWatch}      {this.pulseInterval}");
                this.pulseStopWatch += Time.fixedDeltaTime;
                if (this.pulseStopWatch >= this.pulseInterval)
                {
                    this.pulseStopWatch = 0;
                    //Log.Message("Createing Pulse Explosion");
                    //this.Search(this.candidates);
                    EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
                    {
                        origin = position,
                        scale = radius,
                        rotation = Util.QuaternionSafeLookRotation(Vector3.zero)
                    }, true);
                    this.blastAttack.Fire();
                    //Log.Message("Pulse Fired");
                }
            }
        }
        private void Exit()
        {
            Log.Message("Pulse Over");
        }
        private void Search(List<HurtBox> candidates)
        {
            this.search.origin = this.position;
            this.search.mask = LayerIndex.entityPrecise.mask;
            this.search.radius = this.radius;
            this.search.RefreshCandidates();
            this.search.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(this.teamFilter.teamIndex));
            this.search.OrderCandidatesByDistance();
            this.search.FilterCandidatesByDistinctHurtBoxEntities();
            this.search.GetHurtBoxes(candidates);
            this.search.ClearCandidates();
        }
    }
}
