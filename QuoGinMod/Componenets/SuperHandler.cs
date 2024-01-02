using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using Quo_Gin.Modules;

namespace Quo_Gin.Componenets
{
    internal class SuperHandler : ResourceHandler, IOnKilledOtherServerReceiver
    {
        private int currentOrbOfLightFill;
        private int fillNeededToSpawnOrb = 10;
        private int fillPerElite = 10;
        private int fillPerBoss = 40;
        private int superRefillPerOrb = 20;
        private int maxFillPerKill = 80;

        public override void Start()
        {
            base.Start();
            this.currentOrbOfLightFill = 0;
        }

        public void addOrbFill(float numOfOrbs)
        {
            base.addCurrentResource(numOfOrbs*superRefillPerOrb);
        }
        public void OnKilledOtherServer(DamageReport damageReport)
        {
            if (NetworkServer.active)
            {
                CharacterBody attackerBody = damageReport.attackerBody;
                CharacterBody victimBody = damageReport.victimBody;

                if (attackerBody.teamComponent.teamIndex == base.owner.teamComponent.teamIndex && attackerBody.HasBuff(Buffs.wellOfRadianceBuff)) 
                {
                    Log.Debug("Adding orbFill");
                    this.currentOrbOfLightFill += (int)Mathf.Clamp( 3* Mathf.Log(victimBody.level, 2),1, maxFillPerKill);

                    if (victimBody.isElite) { this.currentOrbOfLightFill += fillPerElite; }
                    if (victimBody.isBoss) { this.currentOrbOfLightFill += fillPerBoss; }


                    Log.Debug("Current orb fill " + this.currentOrbOfLightFill);

                }

                if (this.currentOrbOfLightFill >= this.fillNeededToSpawnOrb)
                {
                    for (int i = this.currentOrbOfLightFill; i >= fillNeededToSpawnOrb; i -= this.fillNeededToSpawnOrb)
                    {
                        Log.Debug("Spawing Orb : ");
                        this.currentOrbOfLightFill -= fillNeededToSpawnOrb;
                        GameObject orbObject = UnityEngine.Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/AmmoPack"), victimBody.gameObject.transform.position, UnityEngine.Random.rotation);
                        UnityEngine.Object.Destroy(orbObject.GetComponent<AmmoPickup>());
                        OrbOfLightPickup orbPickup = orbObject.AddComponent<OrbOfLightPickup>();
                        orbPickup.baseObject = orbObject;
                        orbPickup.teamIndex = attackerBody.teamComponent.teamIndex;
                        orbPickup.owner = this.owner;

                        NetworkServer.Spawn(orbObject);
                    } 
                }
            }
        }

        public static void ApplyOrbOfLight(Collider other)
        {
            if (other.GetComponent<SuperHandler>())
            {
                Log.Debug("has super");
                other.GetComponent<SuperHandler>().addOrbFill(1);
            }
            else
            {
                Log.Debug("no super");
                other.GetComponent<SkillLocator>().ApplyAmmoPack();
            }
        }
    }
}
