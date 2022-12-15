using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public partial class Passive
{
    
    /// <summary>
    /// [ : ] <see cref="Passive"/>
    /// </summary>
    public class Quantum : Passive
    {
        public Quantum(string name) : base(name) { }

        protected override void InternalSetup(bool val)
        {
            if (val)
            {
                GameAction.Move.OnPromptEvent += Effect;
            } else
            {
                GameAction.Move.OnPromptEvent -= Effect;
            }
        }

        private async void Effect(GameAction.Move.PromptArgs args)
        {
            if (args.Performer != EmpoweredPlayer) return;

            if (args is GameAction.Move.PromptArgs.Pathed pathed)
            {
                foreach (var move in await GameAction.Move.PromptSplit(pathed, args.MovingUnit.Allies))
                {

                }
            }
            
            

            args.ReturnCode = -1;
        }

    }

}
