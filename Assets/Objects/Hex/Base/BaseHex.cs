using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// [ : ] <see cref="Hex"/>
/// </summary>
public class BaseHex : Hex, ITeamable
{
    public Team Team { get; private set; }

    public bool IsGuarded
    {
        get
        {
            foreach (Hex hex in _board.HexDict.Values)
            {
                if (hex is not BaseHex bhex) continue;
                if (bhex.Occupant != null && bhex.Occupant.Team == Team) return true;
            }
            return false;
        }
    }

    public void SetTeam(Team team)
    {
        Team = team;
        GetComponent<SpriteRenderer>().color = team.Colors.BaseHex;
    }
}
