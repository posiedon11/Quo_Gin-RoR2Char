using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using RoR2;


namespace Quo_Gin.Componenets
{
    internal class SuperHandler : NetworkBehaviour
    {
        private float superRegenPerSec = .5f;
        private float baseSuperRegenPerSec = .5f;
        private float maxSuper = 100f;
        private float minSuperToCast = 100f;
        private float currentSuper;
        private float bonusSuperRegen;
        private SkillLocator skillLocator;
        private float superReductionSpecial = 3f;
        private float lowestMinSuper = 20f;
        public void Init()
        {
            if (base.hasAuthority)
            {
                this.skillLocator = base.GetComponent<SkillLocator>();
                InitializeSuperHandler();
            }
        }
        private void InitializeSuperHandler()
        {
            this.recalculateMinSuperToCast();
            this.setCurrentSuper(20f);

        }

        public float getMaxSuper()
        {
            return this.maxSuper;
        }
        public float getCurrentSuper() 
        {
            return this.currentSuper;
        }
        public float getMinSuperToCast()
        {
            return this.minSuperToCast;
        }
        public float getTotalSuperRegen()
        {
            return this.superRegenPerSec + this.bonusSuperRegen;
        }
        public float getCurrentSuperRatio()
        {
            return this.currentSuper / this.minSuperToCast;
        }
        public float getMinSuperToCastRatio()
        {
            return this.currentSuper / this.minSuperToCast;
        }
        public void setCurrentSuper(float superVal)
        {
            this.currentSuper = Math.Min(Math.Max(0f, superVal), this.getMaxSuper());
        }
        public void setMinSuperToCast(float superVal) 
        {
            this.minSuperToCast = Math.Min(lowestMinSuper, superVal);
        }
        public void addCurrentSuper(float superVal)
        {
            this.currentSuper = Math.Min(this.currentSuper + superVal, this.maxSuper);
        }
        public void reduceMinSuperToCast(float superVal)
        {
            this.minSuperToCast = Math.Max(lowestMinSuper, minSuperToCast - superVal);
        }

        public void refillSuperMax(float superPercent = 100f)
        {
            this.addCurrentSuper(this.maxSuper * superPercent / 100f);
        }


        private float getMinSuperReductionFromSkill()
        {
            return superReductionSpecial * this.skillLocator.special.maxStock - 1;
        }

        public void recalculateMinSuperToCast()
        {
            if (base.GetComponent<CharacterBody>())
            {
                this.setMinSuperToCast(this.maxSuper - getMinSuperReductionFromSkill());
            }
        }

        public void Hook_Superhandler()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }
        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (base.GetComponent<SuperHandler>() != null)
            {
                base.GetComponent<SuperHandler>().recalculateMinSuperToCast();
            }
        }
    }
}
