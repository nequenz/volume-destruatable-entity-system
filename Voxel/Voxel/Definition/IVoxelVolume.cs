using UnityEngine;
using System;

public interface IVoxelVolume : IVolume<bool>
{
    public const bool Empty = false;
    public const bool Full = true;


    public event Action Matched;


    public IVolumeReadOnly<Color> Colors { get; }

    public IVoxelVolume Init(IVolumeReadOnly<Color> colors, bool canBeReallocated);

    public void SetColors(IVolumeReadOnly<Color> colors);

    public void MatchToColors();
}
