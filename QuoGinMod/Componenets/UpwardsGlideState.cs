using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Quo_Gin.Componenets
{
    internal class UpwardsGlideState  : GenericCharacterMain
    {
        private float glideTimeRemaining;
        private float currentVelocity;
        private float maxGlideTime = 3f;
        private float timeOutTime = 1.5f;
        private float currentTimeOut = 0f;
        private float initialVelocity = .5f;
        private float endVelocity = 0f;
        private float baseYVelocity;
        public static bool inGlide = false;
        public override void OnEnter()
        {
            base.OnEnter();
            inGlide = true;
            //Log.Debug("Entering Upwards Glide");
            this.currentVelocity = initialVelocity;
            this.glideTimeRemaining = maxGlideTime;
            this.baseYVelocity = base.characterMotor.velocity.y;
            this.characterMotor.Motor.ForceUnground();

        }
        public override void HandleMovements()
        {
            base.HandleMovements();
            if (base.isAuthority)
            {
                if (base.inputBank.jump.down)
                {
                    this.currentTimeOut = 0f;
                    this.glideTimeRemaining -= Time.fixedDeltaTime;
                    float yVel = Mathf.SmoothStep(endVelocity, initialVelocity, this.glideTimeRemaining / maxGlideTime);
                    base.characterMotor.velocity.y += yVel;
                }
                this.currentTimeOut += Time.fixedDeltaTime;
            }
            
        }
        public void resetCurrentVelocity()
        {
            baseYVelocity = characterMotor.velocity.y;
        }
        public override void FixedUpdate()
        {
            if (this.currentTimeOut > timeOutTime || this.glideTimeRemaining <= 0)
            {
                Log.Debug("Exiting UpGlide");
                inGlide = false;
                this.outer.SetNextStateToMain();
            }
            base.FixedUpdate();
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
