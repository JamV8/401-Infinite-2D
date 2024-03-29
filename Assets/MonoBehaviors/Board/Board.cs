using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class Board : MonoBehaviour
{
    [SerializeField]
    private Unit _unitPrefab;
    [SerializeField]
    private HXNKey _HXNKey;
    [SerializeField]
    private float _hexSpacing = 0.54f;

    private readonly HashSet<Unit> _units = new();
    private readonly Dictionary<Vector3Int, Hex> _hexDict = new();

    /// <summary>
    /// Gets the set of all Units on the board.
    /// </summary>
    public HashSet<Unit> Units => new(_units);
    public Dictionary<Vector3Int, Hex> HexDict => new(_hexDict);
    public void CreateBoard(Map map, GameSettings settings)
    {
        GenerateMap(map, settings);
        GenerateUnits(map, settings);
    }

    /// <summary>
    /// Instantiates Units on every BaseHex on the board. (GenerateMap() must be called first).
    /// </summary>
    private void GenerateUnits(Map map, GameSettings settings)
    {
        for (int t = 0; t < map.Spawns.Length; t++)
        {
            for (int s = 0; s < map.Spawns[t].Length; s++)
            {
                Unit u = Instantiate(_unitPrefab, transform).Init(this, 3, map.Spawns[t][s]);
                u.SetTeam(settings.Teams[t]);
                u.transform.localPosition = GetLocalTransformAt(u.Position, -1);
                _units.Add(u);
            }
        }
    }

    /// <summary>
    /// Instantiates/renders the physical Hexes of the given <see cref="Map"/>
    /// </summary>
    /// <param name="map"></param>
    /// <param name="hexSpacing"></param>
    private void GenerateMap(Map map, GameSettings settings)
    {
        /*
         The first element(string) of map.HXN starts at 0,0,0(bottom left of map), and then each char in that string generates a Hex at that position.
         each following char generates its Hex 1 position right-downward of the previous, until the end of the string.
         Do this for each element(string) in map.HXN, moving the starting position up 1 hex each time until the entire map is generated.

         (each individual string is a "row", generating from the bottom-up)
         */
        for (int u = 0; u < map.HXN.Length; u++)
        {
            //quick reference var
            string hstr = map.HXN[u];

            //for logging
            //Debug.Log($"generating row: [{hstr}]");

            int o = 0;
            for (int xo = 0; xo < hstr.Length; xo++)
            {
                int x = xo - o;
                Vector3Int coords = (BoardCoords.up * u) - (BoardCoords.left * x);
                Hex hexprefab = _HXNKey.GetHex(hstr[xo]);

                if (hexprefab == null) continue;

                Hex hex = Instantiate(hexprefab, transform).Init(this, coords);
                //Uses helper class BoardCoords 
                hex.transform.localPosition = GetLocalTransformAt(coords);
                _hexDict.Add(coords, hex);

                if (hex is ITeamable thex)
                {
                    thex.SetTeam(settings.Teams[int.Parse(hstr[xo + 1].ToString())]);
                    xo++;
                    o++;
                }

            }


        }

    }

    /// <summary>
    /// Gets the Hex at the given coordinates.
    /// </summary>
    /// <remarks>
    /// If no hex is found at the coordinates: <br></br>
    /// > <paramref name="strict"/> = true : throws an exception. (Default) <br></br>
    /// > <paramref name="strict"/> = false : returns null.
    /// </remarks>
    /// <param name="position"></param>
    /// <returns></returns>
    public Hex HexAt(Vector3Int position, bool strict = false)
    {
        if (!_hexDict.TryGetValue(position, out Hex hex))
        {
            if (strict) throw new System.Exception($"No Hex found at {position} on board {name} | (strict was set true)");
        }
        return hex;
    }

    public HashSet<Hex> HexesAt(IEnumerable<Vector3Int> positions, bool strict = false)
    {
        HashSet<Hex> o = new HashSet<Hex>();

        foreach (Vector3Int pos in positions)
        {
            if (!_hexDict.TryGetValue(pos, out Hex hex))
            {
                if (strict) throw new System.Exception($"No Hex found at {pos} on board {name} | (strict was set true)");
            }
            else
            o.Add(hex);
        }
        return o;
    }

    //ALL changing of a GameObject's in-world position should happen in Board or GameManager. (transform.position should not be used, use transform.localPosition).
    /// <summary>
    /// Gets the transform.localPosition of the specified board coords.
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    public Vector3 GetLocalTransformAt(Vector3Int coords, int zPos = 0)
    {
        Vector2 fpos = coords.CartesianCoordsOf() * _hexSpacing;
        return new Vector3(fpos.x, fpos.y, zPos);
    }

    /// <summary>
    /// [Delegate]
    /// </summary>
    /// <remarks>
    /// <c><see cref="bool"/> ContinuePathCondition(<see cref="Hex"/> <paramref name="prev"/>, <see cref="Hex"/> <paramref name="next"/>) { }</c><br></br>
    /// - <paramref name="prev"/> : The <see cref="Hex"/> that is being stepped off-of during any given step of pathfinding.<br></br>
    /// - <paramref name="next"/> : The <see cref="Hex"/> that is being stepped onto during that same step of pathfinding.<br></br>
    /// <see langword="return"/> -> Whether or not this should be a valid step.
    ///</remarks>
    /// <param name="prev"></param>
    /// <param name="next"></param>
    public delegate bool ContinuePathCondition(Hex prev, Hex next);
    /// <summary>
    /// [Delegate]
    /// </summary>
    /// <remarks>
    /// <c><see cref="int"/> PathWeightFunctionMethod(<see cref="Hex"/> <paramref name="prev"/>, <see cref="Hex"/> <paramref name="next"/>) { }</c><br></br>
    /// - <paramref name="prev"/> : The <see cref="Hex"/> that is being stepped off-of during any given step of pathfinding.<br></br>
    /// - <paramref name="next"/> : The <see cref="Hex"/> that is being stepped onto during that same step of pathfinding.<br></br>
    /// <see langword="return"/> -> The "weight" of this step, or how much range this step subtracts when taken.<br></br>
    /// <i>e.g. A weight of 1 is a regular step, a weight of 2 would mean it would take 2 'steps' just to traverse 1 Hex.</i>
    /// </remarks>
    /// <param name="prev"></param>
    /// <param name="next"></param>
    public delegate int PathWeightFunction(Hex prev, Hex next);
    /// <summary>
    /// [Delegate]
    /// </summary>
    /// <remarks>
    /// <c><see cref="bool"/> FinalPathConditionMethod(<see cref="Hex"/> <paramref name="hex"/>) { }</c><br></br>
    /// - <paramref name="hex"/> : A given <see cref="Hex"/> out of all Hexes that were found during pathfinding.<br></br>
    /// <see langword="return"/> -> whether or not <paramref name="hex"/> should be included in the final output.
    /// </remarks>
    /// <param name="hex"></param>
    public delegate bool FinalPathCondition(Hex hex);

    /// <summary>
    /// Finds all hexes that are within <paramref name="range"/>.min and <paramref name="range"/>.max steps from <paramref name="startPos"/>.<br></br>
    /// Every step must respect the <paramref name="pathCondition"/> and <paramref name="weightFunction"/>, and then found Hexes must pass the <paramref name="finalCondition"/> afterward.
    /// </summary>
    /// <remarks>
    /// (See <see cref="ContinuePathCondition"/>)<br></br>
    /// (See <see cref="FinalPathCondition"/>) <br></br>
    /// (See <see cref="PathWeightFunction"/>) <br></br>
    /// </remarks>
    /// <param name="startPos"></param>
    /// <param name="range"></param>
    /// <param name="pathCondition"></param>
    /// <param name="finalCondition"></param>
    /// <param name="weightFunction"></param>
    ///
    public Dictionary<Hex, int> PathFind(Vector3Int startPos, (int min, int max) range, ContinuePathCondition pathCondition, FinalPathCondition finalCondition, PathWeightFunction weightFunction)
    {
        Dictionary<Hex, int> o = new();
        Dictionary<Hex, int> tickers = new() { { HexAt(startPos), 0} };
        for (int r = 0; r <= range.max;)
        {
            int minWeight = int.MaxValue;
            foreach (Hex h in new List<Hex>(tickers.Keys))
            {
                int val = tickers[h];
                if (val < minWeight) minWeight = val;
                if (val > 0) continue;
                o.Add(h, r);
                foreach(var npos in h.Position.GetAdjacent())
                {
                    if (HexAt(npos) is not Hex nhex) continue;
                    if (o.ContainsKey(nhex)) continue;
                    if (!pathCondition(h, nhex)) continue;
                    int nval = weightFunction(h, nhex);
                    if (!tickers.TryAdd(nhex, nval) && tickers[nhex] > nval)
                        tickers[nhex] = nval;
                }
                tickers.Remove(h);
            }
            foreach (var t in new List<Hex>(tickers.Keys)) tickers[t] -= minWeight;
            r += minWeight;
        }
        foreach (var hex in new List<Hex>(o.Keys))
            if (!finalCondition(hex) || o[hex] < range.min) o.Remove(hex);
        return o;
    }
}
