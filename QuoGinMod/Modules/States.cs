﻿using System.Collections.Generic;
using System;
using Quo_Gin.SkillStates.BaseStates;
using Quo_Gin.SkillStates;
using Quo_Gin.Componenets;

namespace Quo_Gin.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Content.AddEntityState(typeof(BaseMeleeAttack));
            Content.AddEntityState(typeof(ModdedSkillState<SuperHandler>));
            Content.AddEntityState(typeof(SunShot));

            Content.AddEntityState(typeof(AscendingDawn));

            Content.AddEntityState(typeof(WellOfRadiance));

            Content.AddEntityState(typeof(SolarGrenade));
            Content.AddEntityState(typeof(UpwardsGlideState));
            Content.AddEntityState(typeof(GlideState));
        }
    }
}