using EntityStates;
using Quo_Gin.Componenets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quo_Gin.SkillStates.BaseStates
{
    internal class ModdedSkillState<T> : BaseSkillState
    {
        private T resourceHandler;

        public T GetResourceHandler()
        {
            return this.resourceHandler;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Log.Debug("Entering Modded Skill State");
            this.resourceHandler =  base.gameObject.GetComponentInChildren<T>();
        }
    }
}
