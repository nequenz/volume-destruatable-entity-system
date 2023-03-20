using System;
using UnityEngine;


[Serializable]
public sealed class ColorVolume : Volume<Color>
{
    public override Color UndefinedValue => new Color(0.0f, 0.0f, 0.0f, 1.0f);
}