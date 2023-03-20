using System;
using UnityEngine;

[Serializable]
public abstract class Volume<T> : IVolume<T>
{
    private T[,,] _array;
    private Vector3Int _size;
    private bool _canBeReallocated = false;
    private bool _isChangeEventEnabled = true;


    public event Action<Vector3Int> Changed;


    public abstract T UndefinedValue { get; }
    public bool CanBeReallocated => _canBeReallocated;
    public bool IsChangeEventEnabled => _isChangeEventEnabled;
    public Vector3 Size => _size;


    protected bool IsAccessValid(Vector3 position)
    {
        bool zeroCheck = position.x >= 0 && position.y >= 0 && position.z >= 0;
        bool sizeCheck = position.x < _size.x && position.y < _size.y && position.z < _size.z;

        return zeroCheck && sizeCheck;
    }

    public virtual void Allocate() { }

    public virtual void Allocate(Vector3Int size, T initValue)
    {
        if (size.x < 0 || size.y < 0 || size.z < 0)
            throw new ArgumentOutOfRangeException("Wrong params of allocation size");

        _array = new T[size.x, size.y, size.z];
        _size = size;

        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.z; z++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    _array[x, y, z] = initValue;
                }
            }
        }
    }

    public virtual void SetValue(Vector3Int position, T value)
    {
        if (IsAccessValid(position))
        {
            _array[position.x, position.y, position.z] = value;

            if(_isChangeEventEnabled)
                Changed?.Invoke(position);
        }
    }

    public void EnableChangeEvent()
    {
        _isChangeEventEnabled = true;
    }

    public void DisableChangeEvent()
    {
        _isChangeEventEnabled = false;
    }

    public virtual T GetValue(Vector3Int position)
    {
        if (IsAccessValid(position))
            return _array[position.x, position.y, position.z];

        return UndefinedValue;
    }

    public T GetValue(int x, int y, int z) => GetValue(new Vector3Int(x, y, z));

}