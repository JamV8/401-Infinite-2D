using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Selectable
{
    //Change

    public int HP { get; private set; }
    public Player.ETeam Team { get; private set; }
    public int MaxHP { get; private set; }
    public int ID { get; private set; }

    public Vector3Int Position { get; private set; }
    public Board Board => _board;

    protected Board _board;

    private static int _idCount = 0;
    
    

    public Unit Init(Board board, int maxhp, Player.ETeam team, Vector3Int pos)
    {
        _board = board;
        Position = pos;
        Team = team;
        MaxHP = maxhp;
        HP = MaxHP;
        ID = ++_idCount;
        return this;
    }

    public void TestMethod()
    {
        Debug.Log($"I am unit {ID} with {HP} HP.");
    }
    #region Selectable Implementations
    protected override void OnHover(bool state)
    {

    }
    protected override void OnSelectable(bool state)
    {

    }
    protected override void OnSelected()
    {
        TestMethod();
    }
    #endregion
}
