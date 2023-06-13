using System;
using System.Collections.Generic;
using UnityEngine;

public class VoxelBody : MonoBehaviour
{
    public const int NoValue = 0;


    private static readonly List<Vector3> _allocatedVerticies = new();
    private static readonly List<Vector3> _allocatedNormals = new();
    private static readonly List<Vector2> _allocatedUVs = new();
    private static readonly List<Color> _allocatedColors = new();
    private static readonly List<int> _allocatedTriangles = new();
    private static readonly Quaternion _topFaceRotation = Quaternion.identity;
    private static readonly Quaternion _bottomFaceRotation = Quaternion.AngleAxis(180, Vector3.right);
    private static readonly Quaternion _frontFaceRotation = Quaternion.AngleAxis(270, Vector3.right);
    private static readonly Quaternion _backFaceRotation = Quaternion.AngleAxis(90, Vector3.right);
    private static readonly Quaternion _rightFaceRotation = Quaternion.AngleAxis(270, Vector3.forward);
    private static readonly Quaternion _leftFaceRotation = Quaternion.AngleAxis(90, Vector3.forward);
    private static readonly Vector3[] _cubeFaceVerticies =
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(0,0,1),
        new Vector3(1,0,1),
    };
    private static readonly Vector3[] _cubeFaceNormals =
    {
        Vector3.up,
        Vector3.up,
        Vector3.up,
        Vector3.up,
    };
    private static readonly int[] _cubeFaceTriangles =
    {
        0,2,1,
        2,3,1
    };


    public static readonly Vector3Int InvalidSize = new(-1, -1, -1);


    [SerializeField] private bool _isVolumeChangeEventEnabled = true;
    [SerializeField] private MaterialAtlas _materialAtlas;
    [SerializeField] private float _voxelFaceSizeFactor = 0.5f;
    private bool _isMeshDirty = false;
    private bool _isMeshRebuildingSuspended = false;
    private byte[] _volumeValues;
    private Vector3Int _volumeSize;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private MeshCollider _collider;
    private Mesh _mesh;
    private int _lastNewValue;
    private int _lastOldValue;
    private Vector3Int _lastChangedArrayPositon;


    public event Action<Vector3Int, int, int> VolumeChanged;
    public event Action Rebuilt;
    public event Action VertexZeroReached;


    public bool IsChangeEventEnabled => _isVolumeChangeEventEnabled;
    public float VoxelFaceSizeFactor => _voxelFaceSizeFactor;
    public bool IsMeshDirty => _isMeshDirty;
    public bool IsMeshRebuildingSuspended => _isMeshRebuildingSuspended;
    public Vector3 VolumeSize => _volumeSize;
    public int LastNewValue => _lastNewValue;
    public int LastOldValue => _lastOldValue;
    public Vector3Int LastChangedArrayPosition => _lastChangedArrayPositon;
    public MaterialAtlas MaterialAtlas => _materialAtlas;
    public Mesh BuiltMesh => _mesh;
    public MeshCollider MeshCollider => _collider;


    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<MeshCollider>();
    }

    private bool IsAccessValid(Vector3 position)
    {
        bool zeroCheck = position.x >= 0 && position.y >= 0 && position.z >= 0;
        bool sizeCheck = position.x < _volumeSize.x && position.y < _volumeSize.y && position.z < _volumeSize.z;

        return zeroCheck && sizeCheck;
    }

    private int CalculateTo3DIndex(Vector3Int position)
    {
        return CalculateTo3DIndex(position.x, position.y, position.z);
    }

    private int CalculateTo3DIndex(int x, int y, int z)
    {
        return x + _volumeSize.x * (z + _volumeSize.z * y);
    }

    private void AddVoxelFace(Vector3 position, Vector2[] UVs, Quaternion rotation, Quaternion additinalRot)
    {
        const float OffsetZ = 2;

        float _sideCenter = -_voxelFaceSizeFactor / 2;
        Vector3 offset = new(_sideCenter, -_sideCenter, _sideCenter);
        Vector3 validSize = new(_volumeSize.x, _volumeSize.y - OffsetZ, _volumeSize.z);
        Vector3[] vertices = (Vector3[])_cubeFaceVerticies.Clone();
        Vector3[] normals = (Vector3[])_cubeFaceNormals.Clone();
        int[] triangles = (int[])_cubeFaceTriangles.Clone();

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = additinalRot * (rotation * (vertices[i] * _voxelFaceSizeFactor + offset))
                + (position * _voxelFaceSizeFactor) - offset
                - (validSize * _voxelFaceSizeFactor) / 2;
            normals[i] = rotation * normals[i];
        }

        for (int i = 0; i < triangles.Length; i++)
            triangles[i] = triangles[i] + _allocatedVerticies.Count;

        _allocatedVerticies.AddRange(vertices);
        _allocatedNormals.AddRange(normals);
        _allocatedTriangles.AddRange(triangles);
        _allocatedUVs.AddRange(UVs);
    }

    public bool TryAllocate(Vector3Int size, int initValue = NoValue)
    {
        if (_volumeValues is not null || size.x < 0 || size.y < 0 || size.z < 0)
            return false;

        _volumeValues = new byte[(size.x * size.y * size.z)];
        _volumeSize = size;

        if (initValue == 0)
            return true;

        for (int x = 0; x < size.x; x++)
            for (int z = 0; z < size.z; z++)
                for (int y = 0; y < size.y; y++)
                    _volumeValues[CalculateTo3DIndex(x, y, z)] = (byte)initValue;

        return true;
    }

    public void EnableVolumeChangeEvent()
    {
        _isVolumeChangeEventEnabled = true;
    }

    public void DisableVolumeChangeEvent()
    {
        _isVolumeChangeEventEnabled = false;
    }

    public void SetVolumeValue(Vector3Int position, int value)
    {
        int prevValue;

        if (IsAccessValid(position))
        {
            prevValue = _volumeValues[CalculateTo3DIndex(position)];

            if (prevValue != value)
            {
                _volumeValues[CalculateTo3DIndex(position)] = (byte)value;

                if (_isVolumeChangeEventEnabled)
                {
                    VolumeChanged?.Invoke(position, prevValue, value);
                    RebuildMesh();

                    _lastNewValue = value;
                    _lastOldValue = prevValue;
                    _lastChangedArrayPositon = position;
                }
            }
        }
    }

    public int GetVolumeValue(Vector3Int position)
    {
        if (IsAccessValid(position))
            return _volumeValues[CalculateTo3DIndex(position)];

        return NoValue;
    }

    public int GetVolumeValue(int x, int y, int z)
    {
        return GetVolumeValue(new(x, y, z));
    }

    public void SetMaterialAtlas(MaterialAtlas materialAtlas)
    {
        _materialAtlas = materialAtlas;
    }

    public void SetFaceSize(float size)
    {
        _voxelFaceSizeFactor = size;
        _isMeshDirty = true;
    }

    public void RebuildMesh()
    {
        Vector3Int currentPosition = Vector3Int.zero;
        int cubeValue;

        if (_meshFilter is null || _materialAtlas is null)
            return;

        if (_mesh is null)
        {
            _mesh = new();
            _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        _mesh.Clear();
        _allocatedVerticies.Clear();
        _allocatedTriangles.Clear();
        _allocatedNormals.Clear();
        _allocatedUVs.Clear();
        _allocatedColors.Clear();

        for (int x = 0; x < VolumeSize.x; x++)
        {
            for (int z = 0; z < VolumeSize.z; z++)
            {
                for (int y = 0; y < VolumeSize.y; y++)
                {
                    currentPosition.Set(x, y, z);
                    cubeValue = GetVolumeValue(currentPosition);

                    if (cubeValue != NoValue)
                    {
                        if (GetVolumeValue(x, y + 1, z) == NoValue)
                            AddVoxelFace(new Vector3Int(x, y, z),
                                _materialAtlas.GetUV(_materialAtlas.GetCubeInfo(cubeValue).TopUVFace),
                                _topFaceRotation,
                                Quaternion.identity);

                        if (GetVolumeValue(x, y - 1, z) == NoValue)
                            AddVoxelFace(new Vector3Int(x, y, z),
                                _materialAtlas.GetUV(_materialAtlas.GetCubeInfo(cubeValue).BottomUVFace),
                                _bottomFaceRotation,
                                Quaternion.AngleAxis(180, Vector3.up));

                        if (GetVolumeValue(x + 1, y, z) == NoValue)
                            AddVoxelFace(new Vector3Int(x, y, z),
                                _materialAtlas.GetUV(_materialAtlas.GetCubeInfo(cubeValue).RightUVFace),
                                _rightFaceRotation,
                                Quaternion.AngleAxis(270, Vector3.right));

                        if (GetVolumeValue(x - 1, y, z) == NoValue)
                            AddVoxelFace(new Vector3Int(x, y, z),
                                _materialAtlas.GetUV(_materialAtlas.GetCubeInfo(cubeValue).LeftUVFace),
                                _leftFaceRotation,
                                Quaternion.AngleAxis(270, Vector3.right));

                        if (GetVolumeValue(x, y, z - 1) == NoValue)
                            AddVoxelFace(new Vector3Int(x, y, z),
                                _materialAtlas.GetUV(_materialAtlas.GetCubeInfo(cubeValue).FrontUVFace),
                                _frontFaceRotation,
                                Quaternion.identity);

                        if (GetVolumeValue(x, y, z + 1) == NoValue)
                            AddVoxelFace(new Vector3Int(x, y, z),
                                _materialAtlas.GetUV(_materialAtlas.GetCubeInfo(cubeValue).BackUVFace),
                                _backFaceRotation,
                                Quaternion.AngleAxis(180, Vector3.forward));
                    }
                }
            }
        }

        _mesh.vertices = _allocatedVerticies.ToArray();
        _mesh.normals = _allocatedNormals.ToArray();
        _mesh.triangles = _allocatedTriangles.ToArray();
        _mesh.uv = _allocatedUVs.ToArray();
        _mesh.Optimize();

        _meshRenderer.material = _materialAtlas.CurrentMaterial;
        _meshFilter.mesh = _mesh;
        _isMeshDirty = false;

        Rebuilt?.Invoke();

        if (_mesh.vertexCount == 0)
        {
            VertexZeroReached?.Invoke();
            Destroy(gameObject);
        }
        else
        {
            _collider.sharedMesh = _mesh;
        }
    }

    public void SuspendMeshRebuilding()
    {
        _isMeshRebuildingSuspended = true;
    }

    public void ResumeMeshRebuilding()
    {
        _isMeshRebuildingSuspended = false;

        if (_isMeshDirty == true)
            RebuildMesh();
    }

    public Vector3Int CalculateArrayPosition(Vector3 position)
    {
        Vector3Int scaledLocalPosition = default;
        Vector3 offsetPosition = transform.position;
        Vector3 localPosition = Quaternion.Inverse(transform.rotation)
            * (position - offsetPosition)
            + _voxelFaceSizeFactor * (VolumeSize / 2);

        scaledLocalPosition.x = (int)(localPosition.x / _voxelFaceSizeFactor);
        scaledLocalPosition.y = (int)(localPosition.y / _voxelFaceSizeFactor);
        scaledLocalPosition.z = (int)(localPosition.z / _voxelFaceSizeFactor);

        return scaledLocalPosition;
    }

    public Vector3 CalculateWorldPosition(Vector3Int position)
    {
        Vector3 worldPosition = default;

        worldPosition.x = position.x * _voxelFaceSizeFactor;
        worldPosition.y = position.y * _voxelFaceSizeFactor;
        worldPosition.z = position.z * _voxelFaceSizeFactor;
        worldPosition -= ((VolumeSize / 2) * _voxelFaceSizeFactor)
            - new Vector3(_voxelFaceSizeFactor / 2, _voxelFaceSizeFactor / 2, _voxelFaceSizeFactor / 2);
        worldPosition = transform.rotation * worldPosition + transform.position;

        return worldPosition;
    }

    public void SetValueByWorldPosition(Vector3 position, int value)
    {
        SetVolumeValue(CalculateArrayPosition(position), value);
    }

    public int GetValueByWorldPosition(Vector3 position)
    {
        return GetVolumeValue(CalculateArrayPosition(position));
    }
}