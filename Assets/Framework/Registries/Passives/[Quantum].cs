using System;
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

        //TODO: Make an OnPromptEvent sytem similar to OnEvaluateEvent thats all dynamic and async and shit.
        //very good yes nice
        //How is this gunna handle when PromptSplit is called with Units of differing teams? make a PromptArgs.Split perhaps idk.
        private async Task Effect(GameAction.Move.PromptArgs args)
        {
            if (args.Performer != EmpoweredPlayer) return;

            

            args.ReturnCode = -1;
        }

    }

}
