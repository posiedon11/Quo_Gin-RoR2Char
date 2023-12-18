using R2API;
using System;

namespace Quo_Gin.Modules
{
    internal static class Tokens
    {
        internal const string prefix = Quo_GinPlugin.DEVELOPER_PREFIX + "_QUO_GIN_BODY_";
        internal static void AddTokens()
        {
            #region Quo_Gin
            string prefix = Quo_GinPlugin.DEVELOPER_PREFIX + "_QUO_GIN_BODY_";

            string desc = "Henry is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, searching for a new identity.";
            string outroFailure = "..and so he vanished, forever a blank slate.";

            LanguageAPI.Add(prefix + "NAME", "Quo-Gin");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Who Knows");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_PHOENIX_PROTOCOL_NAME", "Phoenix Protocol");
            LanguageAPI.Add(prefix + "PASSIVE_PHEONIX_PROTOCOL_DESCRIPTION",
                $"Reduces skill cooldowns on kills while allies aer inside of Well of Radiance and below " +
                $"<style=cIshealth>{100f * StaticValues.phoenixProtocolHealthProc}% health");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_" + "SUNSHOT" + "_NAME", "SunShot");
            LanguageAPI.Add(prefix + "PRIMARY_" + "SUNSHOT" + "_DESCRIPTION", Helpers.agilePrefix + 
                $"Fires pistol for <style=cIsDamage>{100f * StaticValues.sunShotDamageCoefficient}% damage</style>." +
                $"Kills explode enemies and spread Scorch ");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_" + "SOLAR_GRENADE" + "_NAME", "Solar Grenade");
            LanguageAPI.Add(prefix + "SECONDARY_" + "SOLAR_GRENADE" + "_DESCRIPTION", Helpers.agilePrefix + 
                $"Throws a Lingering Solar Gredade for <style=cIsDamage>{100f * StaticValues.solarGrenadeDamageCoefficient}% damage</style>." +
                $"Damages Enemies iside of radius. ");

            LanguageAPI.Add(prefix + "SECONDARY_" + "CELESTIAL_FIRE" + "_NAME", "Celestial Fire");
            LanguageAPI.Add(prefix + "SECONDARY_" + "CELESTIAL_FIRE" + "_DESCRIPTION", Helpers.agilePrefix + 
                $"Fire a spiral of three Solar blasts for  <style=cIsDamage>{100f * StaticValues.celestialFireDamageCoefficient}% damage</style>. each");

            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_" + "ASCENDING_DAWN" + "_NAME", "Ascending dawn");
            LanguageAPI.Add(prefix + "UTILITY_" + "ASCENDING_DAWN" + "_DESCRIPTION",
                $"Boosts high into the air, damaging all nearby enemies for <style=cIsDamage>{100f * StaticValues.ascendingDawnDamageCoefficient}% damage</style>. " +
                $"refunding secondary ability.");

            LanguageAPI.Add(prefix + "UTILITY_" + "EAGER_EDGE" + "_NAME", "Eager Edge");
            LanguageAPI.Add(prefix + "UTILITY_" + "EAGER_EDGE" + "_DESCRIPTION",
                $"Jump foward with your sword for <style=cIsDamage>{100f * StaticValues.eagerEdgeDamageCoefficient}% damage</style>.. Ignites all enemeis hit.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_" + "WELL_OF_RADIANCE" +"_NAME", "Well of radiance");
            LanguageAPI.Add(prefix + "SPECIAL_" + "WELL_OF_RADIANCE" + "_DESCRIPTION", 
                $"Thrusts sword into ground <style=cIsDamage>{100f * StaticValues.wellOfRadianceDamageCoefficient}% damage</style>, igniting all enemies hit." +
                $"Leave an aura behind that pulses heals for <style=cIsHealing>{100f * StaticValues.wellOfRadianceHealingCoefficient}% health</style>," +
                $" and increases damage to enemies in radius by <style=cIsDamage>{100f * StaticValues.wellOfRadiancedamageMultiplier}% damage</style>/");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Henry: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Henry, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Henry: Mastery");
            #endregion
            #endregion
        }
    }
}