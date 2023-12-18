using Quo_Gin.SkillStates;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace Quo_Gin.Modules
{
    public static class Buffs
    {
        // armor buff gained during roll
        internal static BuffDef armorBuff;
        //passive Buff
        internal static BuffDef pheonixProtocolBuff;

        internal static BuffDef sunShotDebuff;

        internal static void RegisterBuffs()
        {
            armorBuff = AddNewBuff("HenryArmorBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);
            pheonixProtocolBuff = AddNewBuff("QuoGinPheonixProtocolBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);

            sunShotDebuff = AddNewBuff("QuoGinSunShotDebuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                true);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            Content.AddBuffDef(buffDef);

            return buffDef;
        }
    }
}