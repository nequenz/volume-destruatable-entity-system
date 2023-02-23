using System;
using System.Collections;
using UnityEngine;


[Serializable]
public class VoxelVolume : IVoxelVolume
{
    [SerializeField] private bool _canBeReallocated = false;
    private Vector3Int _size;
    private BitArray[,] _bits;
    private IVolumeReadOnly<Color> _prefabToBuild;
    private bool _isChangeEventEnabled = true;

    public event Action<Vector3Int> Changed;
    public event Action Rebuilt;


    public IVolumeReadOnly<Color> PrefabToBuild => _prefabToBuild;
    public bool UndefinedValue => false;
    public bool CanBeReallocated => _canBeReallocated;
    public Vector3 Size => _size;


    private bool IsAccessValid(Vector3 position)
    {
        bool zeroCheck = position.x >= 0 && position.y >= 0 && position.z >= 0;
        bool sizeCheck = position.x < _size.x && position.y < _size.y && position.z < _size.z;

        return zeroCheck && sizeCheck;
    }

    public IVoxelVolume Init(IVolumeReadOnly<Color> colorPrefab, bool canBeReallocated)
    {
        SetVolumePrefabToBuild(colorPrefab);
        _canBeReallocated = canBeReallocated;

        return this;
    }

    public void Allocate()
    {
        Vector3Int size = default;

        if(_prefabToBuild is not null)
        {
            size.x = (int)_prefabToBuild.Size.x;
            size.y = (int)_prefabToBuild.Size.y;
            size.z = (int)_prefabToBuild.Size.z;

            Allocate(size, IVoxelVolume.Empty);
        }
    }

    public void Allocate(Vector3Int size, bool initValue)
    {
        if (size.x < 0 || size.y < 0 || size.z < 0)
            throw new ArgumentOutOfRangeException("Wrong params of allocation size");

        if (_canBeReallocated == false && _bits is not null)
            return;

        _bits = new BitArray[size.x, size.z];
        _size = size;

        for(int x = 0; x < size.x; x ++)
        {
            for (int z = 0; z < size.z; z++)
            {
                _bits[x, z] = new BitArray(size.y);
            }
        }
    }

    public void MatchToPrefab()
    {
        if (_prefabToBuild is null)
            return;

        _isChangeEventEnabled = false;

        for(int x = 0; x < _size.x; x ++)
        {
            for (int z = 0; z < _size.z; z++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    if (_prefabToBuild.GetValue(x, y, z) != _prefabToBuild.UndefinedValue)
                        SetValue(new Vector3Int(x, y, z), IVoxelVolume.Full);
                }
            }
        }

        _isChangeEventEnabled = true;

        Rebuilt?.Invoke();
    }

    public void SetValue(Vector3Int position, bool value)
    {
        bool prevValue;

        if (IsAccessValid(position))
        {
            prevValue = _bits[position.x, position.z].Get(position.y);

            if(prevValue != value)
            {
                _bits[position.x, position.z].Set(position.y, value);

                if(_isChangeEventEnabled)
                    Changed?.Invoke(position);
            }
        }
    }

    public void SetVolumePrefabToBuild(IVolumeReadOnly<Color> colorPrefab) => _prefabToBuild = colorPrefab;

    public bool GetValue(Vector3Int position)
    {
        if (IsAccessValid(position))
            return _bits[position.x, position.z].Get(position.y);

        return UndefinedValue;
    }

    public bool GetValue(int x, int y, int z) => GetValue(new Vector3Int(x, y, z));

}
