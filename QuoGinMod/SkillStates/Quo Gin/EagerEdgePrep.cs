using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using R2API;
using UnityEngine;
using EntityStates;


namespace Quo_Gin.SkillStates
{
    internal class EagerEdgePrep : BaseState
    {
        private float duration;
        private float baseDuration = .5f;


        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration / this.attackSpeedStat;
            base.PlayAnimation("FullBody, Override", "AssaulterPrep", "AssaulterPrep.playbackRate", this.duration);
            GameObject gameObject = base.FindModelChildGameObject("PreDashEffect");
            if (gameObject != null)
            {
                gameObject.SetActive(true);
            }

            Ray aimRay = base.GetAimRay();
            base.characterDirection.forward = aimRay.direction;
            base.characterDirection.moveVector = aimRay.direction;
            base.SmallHop(base.characterMotor, 3f);
        }

        public override void OnExit() 
        {
            GameObject gameObject = base.FindModelChildGameObject("PreDashEffect");
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > this.duration)
            {
                this.outer.SetNextState(new EagerEdgeLunge());
                //this.outer.SetNextStateToMain();
            }
        }
    }
}
