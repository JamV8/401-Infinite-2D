using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public static class PassiveRegistry
{
    public static ReadOnlyCollection<ConstructionTemplate<Passive>> Registry { get; private set; }


    public static void Initialize(GameSettings settings)
    {
        List<ConstructionTemplate<Passive>> masterList = new()
        {
            () => new Passive.Agile(),
            () => new Passive.PointRunner(),
            () => new Passive.Quantum()
        };

        Registry = masterList.AsReadOnly();
    }
}
