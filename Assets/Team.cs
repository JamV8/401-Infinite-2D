using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Team
{
    public string Name { get; private set; }
    public Pallete Colors { get; private set; }
    public int PerspectiveRotation { get; private set; }

    public Team(string name, Color mainColor, int perspective)
    {
        Name = name;
        Colors = new(mainColor);
        PerspectiveRotation = perspective;
    }

    public struct Pallete
    {
        public Color Primary { get; private set; }
        public Color Unit { get; private set; }
        public Color BaseHex { get; private set; }

        public Pallete(Color primary)
        {
            //placeholder colors for now
            Primary = primary;
            Unit = Color.Lerp(primary, Color.white, 0.3f);
            BaseHex = Color.Lerp(primary, Color.white, 0.1f);
        }
    }
    public override string ToString() => Name;
}