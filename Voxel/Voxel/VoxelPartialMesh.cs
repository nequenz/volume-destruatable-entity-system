using UnityEngine;
using System;
using System.Collections.Generic;

public struct MeshChunk
{
    private Vector3[] _verticies;
    private Vector3[] _normals;
    private int[] _triangles;
    private Color[] _colors;


    public Vector3[] Verticies => _verticies;
    public Vector3[] Normals => _normals;
    public int[] Triangles => _triangles;
    public Color[] Colors => _colors;
}


public class VoxelPartialMesh
{
    private int _chunkMeshSize = 3;
    private MeshChunk[,,] _chunks;
    private Vector3Int _chunksSize;


    public void AllocateMeshChunks(IVoxelVolume volume)
    {
        int x, y, z;

        if (volume is null || volume.Size.magnitude == 0.0f)
            return;

        x = Mathf.CeilToInt(volume.Size.x / _chunkMeshSize);
        z = Mathf.CeilToInt(volume.Size.z / _chunkMeshSize);
        y = Mathf.CeilToInt(volume.Size.y / _chunkMeshSize);

        _chunks = new MeshChunk[x, z, y];
    }

    public void RebuildChunkMesh(Vector3Int volumePosition)
    {
        int x, y, z;

        x = Mathf.FloorToInt(volumePosition.x / _chunkMeshSize);
        z = Mathf.FloorToInt(volumePosition.z / _chunkMeshSize);
        y = Mathf.FloorToInt(volumePosition.y / _chunkMeshSize);


    }
}