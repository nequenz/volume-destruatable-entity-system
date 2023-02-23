using UnityEngine;
using System;

public interface IVoxelVolume : IVolume<bool>
{
    public const bool Empty = false;
    public const bool Full = true;


    public event Action Rebuilt;


    public IVolumeReadOnly<Color> PrefabToBuild { get; }


    public IVoxelVolume Init(IVolumeReadOnly<Color> prefabToBuild, bool canBeReallocated);

    public void SetVolumePrefabToBuild(IVolumeReadOnly<Color> prefabToBuild);

    public void MatchToPrefab();
}
