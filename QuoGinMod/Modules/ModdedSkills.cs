using EntityStates;
using JetBrains.Annotations;
using Quo_Gin.Componenets;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Quo_Gin.Modules
{
    internal class ModdedSkillDefInfo
    {
        public string skillName;
        public string skillNameToken;
        public string skillDescriptionToken;
        public string[] keywordTokens = new string[0];
        public Sprite skillIcon;

        public SerializableEntityStateType activationState;
        public InterruptPriority interruptPriority;
        public string activationStateMachineName;

        public float baseRechargeInterval;

        public int baseMaxStock = 1;
        public int rechargeStock = 1;
        public int requiredStock = 1;
        public int stockToConsume = 1;

        public bool isCombatSkill = true;
        public bool canceledFromSprinting;
        public bool forceSprintDuringState;
        public bool cancelSprintingOnActivation = true;

        public bool beginSkillCooldownOnSkillEnd;
        public bool fullRestockOnAssign = true;
        public bool resetCooldownTimerOnUse;
        public bool mustKeyPress;
        public float baseResourceCost = 0f;
        public float percentageCost = 0f;
        public float percentToDrain = 0f;
    }
    internal class moddedSkillDef : SkillDef
    {

        protected class CharacterModInstanceData : SkillDef.BaseSkillInstanceData
        {
            public ResourceHandler resourceHandler;
        }
        public float baseResourceCost;
        public float percentResourcecost;
        public float percentageToDrain;

        public static moddedSkillDef CreateModdedSkillDef(ModdedSkillDefInfo skillDefInfo)
        {
            return CreateModdedSkillDef<moddedSkillDef>(skillDefInfo);

        }
        public static T CreateModdedSkillDef<T>(ModdedSkillDefInfo skillDefInfo) where T : moddedSkillDef
        {
            //pass in a type for a custom skilldef, e.g. HuntressTrackingSkillDef
            T skillDef = ScriptableObject.CreateInstance<T>();

            skillDef.skillName = skillDefInfo.skillName;
            (skillDef as ScriptableObject).name = skillDefInfo.skillName;
            skillDef.skillNameToken = skillDefInfo.skillNameToken;
            skillDef.skillDescriptionToken = skillDefInfo.skillDescriptionToken;
            skillDef.icon = skillDefInfo.skillIcon;

            skillDef.activationState = skillDefInfo.activationState;
            skillDef.activationStateMachineName = skillDefInfo.activationStateMachineName;
            skillDef.baseMaxStock = skillDefInfo.baseMaxStock;
            skillDef.baseRechargeInterval = skillDefInfo.baseRechargeInterval;
            skillDef.beginSkillCooldownOnSkillEnd = skillDefInfo.beginSkillCooldownOnSkillEnd;
            skillDef.canceledFromSprinting = skillDefInfo.canceledFromSprinting;
            skillDef.forceSprintDuringState = skillDefInfo.forceSprintDuringState;
            skillDef.fullRestockOnAssign = skillDefInfo.fullRestockOnAssign;
            skillDef.interruptPriority = skillDefInfo.interruptPriority;
            skillDef.resetCooldownTimerOnUse = skillDefInfo.resetCooldownTimerOnUse;
            skillDef.isCombatSkill = skillDefInfo.isCombatSkill;
            skillDef.mustKeyPress = skillDefInfo.mustKeyPress;
            skillDef.cancelSprintingOnActivation = skillDefInfo.cancelSprintingOnActivation;
            skillDef.rechargeStock = skillDefInfo.rechargeStock;
            skillDef.requiredStock = skillDefInfo.requiredStock;
            skillDef.stockToConsume = skillDefInfo.stockToConsume;

            skillDef.keywordTokens = skillDefInfo.keywordTokens;
            skillDef.baseResourceCost = skillDefInfo.baseResourceCost;
            skillDef.percentResourcecost = skillDefInfo.percentageCost;
            skillDef.percentageToDrain = skillDefInfo.percentToDrain;
            Content.AddSkillDef(skillDef);


            return skillDef;
        }
        public override void OnExecute([NotNull] GenericSkill skillSlot)
        {
            Log.Debug("Executing Resource Skill");
            base.OnExecute(skillSlot);
            if (this.GetResourceHandler(skillSlot) != null && skillSlot.characterBody != null)
            {
                float resourceCost = GetCalculatedResourceCost(this.baseResourceCost, skillSlot.cooldownScale);
                float percentCost = GetCalculatedResourceCost(this.percentResourcecost, skillSlot.cooldownScale);

                if (resourceCost != 0f)
                {
                    Log.Debug($"Removig Resource: {(resourceCost)}");
                    this.GetResourceHandler(skillSlot).addCurrentResource(-resourceCost);
                    Log.Debug($"Current Resource: {(this.GetResourceHandler(skillSlot).getCurrentResource())}");
                }
                if (percentCost > 0f)
                {
                    Log.Debug($"Removig Percentage: {(percentCost)} from {this.GetResourceHandler(skillSlot).getCurrentResource()}");
                    this.GetResourceHandler(skillSlot).addCurrentResource(-(percentCost * this.GetResourceHandler(skillSlot).getMaxResource()));
                    Log.Debug($"Current Resource: {(this.GetResourceHandler(skillSlot).getCurrentResource())}");
                }
            }
        }

        public static float GetCalculatedResourceCost(float value, float cooldown)
        {
            if (value != 0f)
            {
                return Mathf.Max(1f, value * cooldown);
            }
            return 0f;
        }

        private ResourceHandler GetResourceHandler([NotNull] GenericSkill skillSlot)
        {
            return this.GetCharacterModInstance(skillSlot).resourceHandler;
        }
        private moddedSkillDef.CharacterModInstanceData GetCharacterModInstance([NotNull] GenericSkill skillSlot)
        {
            return (moddedSkillDef.CharacterModInstanceData)skillSlot.skillInstanceData;
        }
        public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new moddedSkillDef.CharacterModInstanceData
            {
                resourceHandler = skillSlot.GetComponent<ResourceHandler>()
            };
        }


        protected virtual bool CanAfford([NotNull] GenericSkill skillSlot)
        {
            if (this.GetResourceHandler(skillSlot).isReducedResource())
            {
                return this.GetResourceHandler(skillSlot) != null && this.canAffordMinCost(skillSlot);
            }
            else
            {
                return this.GetResourceHandler(skillSlot) != null && this.canAffordBasecost(skillSlot);
            }
        }

        protected bool canAffordBasecost([NotNull] GenericSkill skillSlot)
        {
            float calcCost = GetCalculatedResourceCost(this.baseResourceCost, skillSlot.cooldownScale);
            return this.GetResourceHandler(skillSlot).getCurrentResource() >= this.GetResourceHandler(skillSlot).getMaxResource();
        }
        protected bool canAffortPercentcost([NotNull] GenericSkill skillSlot)
        {
            float calcCost = GetCalculatedResourceCost(this.percentResourcecost, skillSlot.cooldownScale);
            return this.GetResourceHandler(skillSlot).getCurrentResource() >= this.GetResourceHandler(skillSlot).getMaxResource() * calcCost;
        }

        protected bool canAffordMinCost([NotNull] GenericSkill skillSlot)
        {
            return this.GetResourceHandler(skillSlot).getCurrentResource() >= this.GetResourceHandler(skillSlot).getMinResourceReduction(3);
        }

        protected virtual bool CanUseSkill([NotNull] GenericSkill skillSlot)
        {
            return this.CanAfford(skillSlot);
        }
        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            //return base.IsReady(skillSlot) && this.;

            return base.IsReady(skillSlot) && this.CanUseSkill(skillSlot);
            //return false;
        }



    }
}
