using UnityEngine;


public interface IVoxelTransform
{
    public Transform AttachedTransform { get; }
    public IVoxelVolume AttachedVoxelVolume { get; }
    public IVoxelMesh AttachedVoxelMesh { get; }
    public MeshCollider AttachedMeshCollider { get; }


    public IVoxelTransform Init(Transform transform, IVoxelVolume volume, IVoxelMesh mesh, MeshCollider collider);

    public void SetTransform(Transform transform);

    public void SetVolume(IVoxelVolume volume);

    public void SetVolumeMesh(IVoxelMesh mesh);

    public void SetMeshCollider(MeshCollider collider);

    public Vector3Int CalculateVoxelPosition(Vector3 position);

    public Vector3 CalculateWorldPosition(Vector3Int position);
}