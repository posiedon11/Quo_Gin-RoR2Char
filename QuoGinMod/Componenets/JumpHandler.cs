using EntityStates;
using Quo_Gin.Modules.Characters;
using Quo_Gin.Modules.Survivors;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Quo_Gin.Componenets
{
    internal class JumpHandler : NetworkBehaviour
    {
        private CharacterBody owner;
        private CharacterMotor motor;
        private InputBankTest inputBank;
        private EntityStateMachine stateMachine;

        public static bool startedGlide;
        public void Awake()
        {
            this.owner = base.GetComponentInChildren<CharacterBody>();
            this.motor = base.GetComponentInChildren<CharacterMotor>();
            this.inputBank = base.GetComponentInChildren<InputBankTest>();
            this.stateMachine = base.GetComponentInChildren<EntityStateMachine>();
            startedGlide = false;
        }

        public void Update()
        {
            if (base.hasAuthority)
            {
                ProcessJump();
                
            }
        }
        public void ProcessJump()
        {
            if (this.motor && this.inputBank)
            {
                if (this.inputBank.jump.down & this.motor.jumpCount == 1)
                {
                    this.motor.jumpCount--;
                    Log.Debug("Starting Glide Jump");
                    startedGlide = true;
                    this.stateMachine.SetNextState(new UpwardsGlideState());
                }
            }
            
        }
    }
}
