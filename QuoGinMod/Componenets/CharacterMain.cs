using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quo_Gin.Componenets
{
    internal class CharacterMain : GenericCharacterMain
    {
        private bool canGlide;
        public override void ProcessJump()
        {
            base.ProcessJump();
            if (base.isGrounded)
            {
                canGlide = true;
            }
            if (this.hasCharacterMotor && this.hasInputBank)
            {
                if (base.inputBank.jump.down & this.characterMotor.jumpCount == this.characterBody.maxJumpCount & canGlide)
                {
                    canGlide = false;
                    this.outer.SetNextState(new UpwardsGlideState());
                }
            }

        }
    }
}
