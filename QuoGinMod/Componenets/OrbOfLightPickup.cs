using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Quo_Gin.Componenets
{
    internal class OrbOfLightPickup: MonoBehaviour
    {
        public GameObject baseObject;
        public CharacterBody owner;
        public TeamIndex teamIndex;
        public bool alive = true;
        private void OnTriggerStay(Collider other)
        {
            if (NetworkServer.active && this.alive && TeamComponent.GetObjectTeam(other.gameObject) == this.teamIndex)
            {
                if (other.GetComponent<SkillLocator>())
                {
                    Log.Debug("Someone touched the orb");
                    this.alive = false;
                    SuperHandler.ApplyOrbOfLight(other);
                    UnityEngine.Object.Destroy(this.baseObject);
                }
            }
        }
    }
}
