using UnityEngine;


public interface IVolumeReadOnly<T>
{
    public T UndefinedValue { get; }
    public bool CanBeReallocated { get; }
    public Vector3 Size { get; }


    public T GetValue(Vector3Int position);

    public T GetValue(int x, int y, int z);
}