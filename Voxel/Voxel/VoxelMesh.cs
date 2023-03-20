using System;
using UnityEngine;


[Serializable]
public class VoxelMesh : IVoxelMesh
{
    [SerializeField] private float _sizeFactor = 1f;
    [SerializeField] private MeshFilter _filter;
    private IVolumeReadOnly<Color> _colors;
    private IVoxelVolume _voxels;
    private Mesh _mesh;
    private float _currentDelay = 0.0f;
    private bool _isDirty = false;
    private bool _isRebuildingSuspended = false;


    public event Action Rebuilt;


    public IVoxelVolume AttachedVoxelVolume => _voxels;
    public IVolumeReadOnly<Color> AttachedColorVolume => _colors;
    public Mesh BuiltMesh => _mesh;
    public float FaceSize => _sizeFactor;
    public bool IsDirty => _isDirty;


    private void OnVoxelsChanged(Vector3Int position)
    {
        if(_isRebuildingSuspended == false)
            RebuildForced();
    }

    private void AddFace(Vector3 position, Quaternion rotation, Color color)
    {
        const float OffsetZ = 2;

        float _sideCenter = -_sizeFactor / 2;
        Vector3 offset = new (_sideCenter, -_sideCenter, _sideCenter);
        Vector3 validSize = new (_voxels.Size.x, _voxels.Size.y - OffsetZ, _voxels.Size.z);
        Vector3[] vertices = new Vector3[VoxelMeshInfo.SideVertexCount];
        Vector3[] normals = { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
        int[] triangles = new int[VoxelMeshInfo.SideTriangleCount];

        VoxelMeshInfo.SetVertices(ref vertices);
        VoxelMeshInfo.SetTriangles(ref triangles);

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = (rotation * (vertices[i] * _sizeFactor + offset))
                + (position * _sizeFactor) - offset
                - (validSize * _sizeFactor) / 2;
            normals[i] = rotation * normals[i];
        }

        for (int i = 0; i < triangles.Length; i++)
            triangles[i] = triangles[i] + MeshAllocator.VertexCount;

        MeshAllocator.AddVertices(vertices);
        MeshAllocator.AddNormals(normals);
        MeshAllocator.AddTriangles(triangles);
        MeshAllocator.AddColors(color, VoxelMeshInfo.SideVertexCount);
    }

    public IVoxelMesh Init(IVoxelVolume voxels, IVolumeReadOnly<Color> colors, MeshFilter filter, float delay, float size)
    {
        SetVoxelVolume(voxels);
        SetColorVolume(colors);
        SetMeshFilter(filter);
        SetSize(size);

        return this;
    }

    public void SetVoxelVolume(IVoxelVolume voxels)
    {
        if (voxels is null)
            return;

        _voxels = voxels;
        _isDirty = true;
        _voxels.Changed += OnVoxelsChanged;
    }

    public void SetColorVolume(IVolumeReadOnly<Color> colors)
    {
        _colors = colors;
        _isDirty = true;
    }

    public void SetMeshFilter(MeshFilter filter)
    {
        _filter = filter;
    }

    public void SetSize(float size)
    {
        _sizeFactor = size;
        _isDirty = true;
    }

    public void SuspendRebuilding()
    {
        _isRebuildingSuspended = true;
    }

    public void ResumeRebuilding()
    {
        _isRebuildingSuspended = false;
    }

    public void RebuildForced()
    {
        Vector3Int currentPosition = Vector3Int.zero;
        bool voxelValue;

        if (_colors is null || _voxels is null || _filter is null)
            return;

        if (_mesh is null)
        {
            _mesh = new();
            _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }
            

        _mesh.Clear();
        MeshAllocator.Clear();

        for (int x = 0; x < _voxels.Size.x; x++)
        {
            for (int z = 0; z < _voxels.Size.z; z++)
            {
                for (int y = 0; y < _voxels.Size.y; y++)
                {
                    currentPosition.Set(x, y, z);
                    voxelValue = _voxels.GetValue(currentPosition);

                    if (voxelValue == IVoxelVolume.Full)
                    {
                        if (_voxels.GetValue(x, y + 1, z) == IVoxelVolume.Empty)
                            AddFace(new Vector3Int(x, y, z), VoxelMeshInfo.TopSide, _colors.GetValue(currentPosition));

                        if (_voxels.GetValue(x, y - 1, z) == IVoxelVolume.Empty)
                            AddFace(new Vector3Int(x, y, z), VoxelMeshInfo.BottomSide, _colors.GetValue(currentPosition));

                        if (_voxels.GetValue(x + 1, y, z) == IVoxelVolume.Empty)
                            AddFace(new Vector3Int(x, y, z), VoxelMeshInfo.RightSide, _colors.GetValue(currentPosition));

                        if (_voxels.GetValue(x - 1, y, z) == IVoxelVolume.Empty)
                            AddFace(new Vector3Int(x, y, z), VoxelMeshInfo.LeftSide, _colors.GetValue(currentPosition));

                        if (_voxels.GetValue(x, y, z - 1) == IVoxelVolume.Empty)
                            AddFace(new Vector3Int(x, y, z), VoxelMeshInfo.FrontSide, _colors.GetValue(currentPosition));

                        if (_voxels.GetValue(x, y, z + 1) == IVoxelVolume.Empty)
                            AddFace(new Vector3Int(x, y, z), VoxelMeshInfo.BackSide, _colors.GetValue(currentPosition));
                    }
                }
            }
        }

        _mesh.vertices = MeshAllocator.CloneVertices();
        _mesh.normals = MeshAllocator.CloneNormals();
        _mesh.triangles = MeshAllocator.CloneTriangles();
        _mesh.colors = MeshAllocator.CloneColors();
        _mesh.Optimize();

        _filter.mesh = _mesh;
        _isDirty = false;

        Rebuilt?.Invoke();
    }

    public void Update(float deltaTime)
    {
        if( _currentDelay > 0.0f )
        {
            _currentDelay -= deltaTime;

            if(_currentDelay <= 0.0f)
                RebuildForced();
        }
    }

}