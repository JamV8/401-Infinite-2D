using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class GameAction
{

    public class Turn : GameAction
    {
        public static event GameActionEventHandler<Turn> OnPerform;

        public Player FromPlayer { get; private set; }
        public Player ToPlayer { get; private set; }

        protected override void InternalPerform()
        {
            //Handled by GameManager
            OnPerform?.Invoke(this);
        }

        protected override void InternalUndo()
        {
            //Handled by GameManager
        }

        private Turn(Player fromPlayer, Player toPlayer) : base(fromPlayer)
        {
            FromPlayer = fromPlayer;
            ToPlayer = toPlayer;
        }

        public static void Declare(Player fromPlayer, Player toPlayer)
        {
            GameManager.GAME.PushGameAction(new Turn(fromPlayer, toPlayer));
        }
    }

}