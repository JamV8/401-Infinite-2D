using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public static class AbilityRegistry
{

    public static ReadOnlyCollection<ConstructorTemplate<Ability>> Registry { get; private set; }
    
    static AbilityRegistry()
    {
        /*
        * Sourced:
        * NAME - string
        * TYPE IDENTITY - Ability.ETypeIdentity
        * TARGET EFFECTS - ConstructorTemplate<UnitEffect>[]
        * HIT AREA - HashSet<Vector3Int>
        * TARGETING CONDITIONS - Ability.Sourced.TargetingCondition[]
        * 
        * Unsourced:
        * NAME - string
        * TYPE IDENTITY - Ability.ETypeIdentity
        * ON-PLAY ACTION - Action<GameAction.PlayAbility> actionMethod
        * INITIAL TARGET CONDITION - Ability.Unsourced.SingleTargetCondition
        * (May be excluded) SECONDARY TARGET CONDITIONS  - Ability.Unsourced.TargetCondition[]
        */

        List<ConstructorTemplate<Ability>> masterList = new()
        {
            //>0 TEST ATTACK
            new
            (
                typeof(Ability.Sourced),

                "Test Attack", Ability.ETypeIdentity.Attack,

                new ConstructorTemplate<UnitEffect>[]
                {
                    new(typeof(UnitEffect.Slowed), 1)
                },
                new HashSet<Vector3Int>
                {
                    BoardCoords.up
                },
                new Ability.Sourced.TargetingCondition[]
                {
                    Ability.Sourced.STANDARD_ATTACK,
                    Ability.Sourced.STANDARD_COLLISION
                }
            ),

            //>1 TEST UTILITY
            new
            (
                typeof(Ability.Unsourced),

                "Test Utility", Ability.ETypeIdentity.Utility,

                new Action<GameAction.PlayAbility>(a =>
                {
                    GameAction.Move.Prompt(new GameAction.Move.PromptArgs.Pathed(a.Performer, a.ParticipatingUnits[0], 8),
                        move => a.AddLateResultant(move));
                }),

                new Ability.Unsourced.SingleTargetCondition((p, u) => p.Team == u.Team)
            ),
        };


        //FINALIZE REGISTRY
        Registry = new ReadOnlyCollection<ConstructorTemplate<Ability>>(masterList);
    }

}