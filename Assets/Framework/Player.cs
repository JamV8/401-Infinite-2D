using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{

    //main team enum
    public enum ETeam : byte
    {
        Blue,
        Red
    }

    //Player class stores a player's Abilities, Passive, Control spheres, etc.
    //Under normal circumstances there will only be 2 player instances (Blue player and Red player).
    //Players are *not* Units. Units are the peices that move across the board that have HP, status effects, etc. , while Player objects hold information about the entire side, such as the cards, passive, etc. a player has.

}