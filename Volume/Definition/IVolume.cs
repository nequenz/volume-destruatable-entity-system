using System;
using UnityEngine;



public interface IVolume<T>
{
    public event Action<Vector3Int> Changed;


    public T UndefinedValue { get; }
    public bool CanBeReallocated { get; }
    public Vector3 Size { get; }
    public bool IsChangeEventEnabled { get; }


    public void Allocate();

    public void Allocate(Vector3Int size, T initValue);

    public void SetValue(Vector3Int position, T value);

    public void EnableChangeEvent();

    public void DisableChangeEvent();

    public T GetValue(Vector3Int position);

    public T GetValue(int x, int y, int z);
}