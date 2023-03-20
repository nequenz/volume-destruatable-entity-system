using UnityEngine;
using System;

public static class VoxelMeshInfo
{
    public const int SideVertexCount = 4;
    public const int SideTriangleCount = 6;

    private static readonly Vector3[] _verticies =
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(0,0,1),
        new Vector3(1,0,1),
    };
    private static readonly Vector3[] _normals =
    {
        Vector3.up,
        Vector3.up,
        Vector3.up,
        Vector3.up,
    };
    private static readonly int[] _triangles =
    {
        0,2,1,
        2,3,1
    };


    public static readonly Quaternion TopSide = Quaternion.identity;
    public static readonly Quaternion BottomSide = Quaternion.AngleAxis(180, Vector3.right);
    public static readonly Quaternion FrontSide = Quaternion.AngleAxis(270, Vector3.right);
    public static readonly Quaternion BackSide = Quaternion.AngleAxis(90, Vector3.right);
    public static readonly Quaternion RightSide = Quaternion.AngleAxis(270, Vector3.forward);
    public static readonly Quaternion LeftSide = Quaternion.AngleAxis(90, Vector3.forward);


    public static void SetVertices(ref Vector3[] other)
    {
        if (other is null || other.Length != SideVertexCount)
            throw new IndexOutOfRangeException("Voxel side mesh must have " + SideVertexCount + " vertices!");

        for (int i = 0; i < other.Length; i++)
            other[i] = _verticies[i];
    }

    public static void SetNormals(ref Vector3[] other)
    {
        if (other is null || other.Length != SideVertexCount)
            throw new IndexOutOfRangeException("Voxel side mesh must have " + SideVertexCount + " normals!");

        for (int i = 0; i < other.Length; i++)
            other[i] = _normals[i];
    }

    public static void SetTriangles(ref int[] other)
    {
        if (other is null || other.Length != SideTriangleCount)
            throw new IndexOutOfRangeException("Voxel side mesh must have " + SideTriangleCount + " indices!");

        for (int i = 0; i < other.Length; i++)
            other[i] = _triangles[i];
    }
}