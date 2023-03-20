using System;
using UnityEngine;


public class VolumeReadOnly<T> : IVolumeReadOnly<T>
{
    private IVolume<T> _volume;


    public Vector3 Size => _volume.Size;
    public T UndefinedValue => _volume.UndefinedValue;
    public bool CanBeReallocated => _volume.CanBeReallocated;


    public VolumeReadOnly(IVolume<T> volume)
    {
        _volume = volume;
    }

    public T GetValue(Vector3Int position) => _volume.GetValue(position);

    public T GetValue(int x, int y, int z) => _volume.GetValue(x, y, z);
}