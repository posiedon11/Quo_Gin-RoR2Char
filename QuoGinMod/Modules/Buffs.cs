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
        internal static BuffDef PhoenixProtocolBuff;

        internal static BuffDef sunShotDebuff;
        internal static BuffDef ascendingDawnJumpBuff;
        internal static BuffDef ascendingDawnKillBuff;

        internal static BuffDef wellOfRadianceBuff;
        internal static BuffDef scorchDebuff;
        internal static void RegisterBuffs()
        {
            armorBuff = AddNewBuff("HenryArmorBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);
            PhoenixProtocolBuff = AddNewBuff("QuoGinPhoenixProtocolBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.yellow,
                false,
                false);

            sunShotDebuff = AddNewBuff("QuoGinSunShotDebuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.black,
                false,
                true);
            ascendingDawnJumpBuff = AddNewBuff("QuoGinAscendingDawnJumpBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.green,
                false,
                false);
            ascendingDawnKillBuff = AddNewBuff("QuoGinAscendingDawnKillBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.blue,
                false,
                false);
            scorchDebuff = AddNewBuff("QuoGinScorchDeBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.red,
                true,
                true);
            wellOfRadianceBuff = AddNewBuff("QuoGinWellBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.cyan,
                false,
                false);


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