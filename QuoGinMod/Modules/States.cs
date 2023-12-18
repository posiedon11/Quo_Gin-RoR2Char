using System.Collections.Generic;
using System;
using Quo_Gin.SkillStates.BaseStates;
using Quo_Gin.SkillStates;

namespace Quo_Gin.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Content.AddEntityState(typeof(BaseMeleeAttack));
            Content.AddEntityState(typeof(SunShot));

            Content.AddEntityState(typeof(AscendingDawn));

            Content.AddEntityState(typeof(WellOfradiance));

            Content.AddEntityState(typeof(SolarGrenade));
        }
    }
}