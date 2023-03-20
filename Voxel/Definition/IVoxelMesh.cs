using UnityEngine;
using System;


public interface IVoxelMesh
{
    public event Action Rebuilt;


    public IVolumeReadOnly<Color> AttachedColorVolume { get; }
    public IVoxelVolume AttachedVoxelVolume { get; }
    public Mesh BuiltMesh { get; }
    public float FaceSize { get; }
    public bool IsDirty { get; }


    public IVoxelMesh Init(IVoxelVolume voxels, IVolumeReadOnly<Color> colors, MeshFilter filter, float delay, float size);

    public void SetVoxelVolume(IVoxelVolume voxels);

    public void SetColorVolume(IVolumeReadOnly<Color> colors);

    public void SetMeshFilter(MeshFilter filter);

    public void SuspendRebuilding();

    public void ResumeRebuilding();

    public void RebuildForced();

    public void SetSize(float size);

    public void Update(float deltaTime);
}
