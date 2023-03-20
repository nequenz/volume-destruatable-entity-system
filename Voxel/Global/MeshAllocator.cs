using UnityEngine;
using System.Collections.Generic;

public static class MeshAllocator
{
    private static readonly List<Vector3> _verticies = new();
    private static readonly List<Vector3> _normals = new();
    private static readonly List<Vector2> _uvs = new();
    private static readonly List<int> _triangles = new();
    private static readonly List<Color> _colors = new();


    public static int VertexCount => _verticies.Count;
    public static int NormalCount => _normals.Count;
    public static int UVCount => _uvs.Count;
    public static int TriangleCount => _triangles.Count;
    public static int ColorCount => _colors.Count;


    public static void Clear()
    {
        _verticies.Clear();
        _normals.Clear();
        _uvs.Clear();
        _triangles.Clear();
        _colors.Clear();
    }

    public static void AddVertices(Vector3[] other)
    {
        _verticies.AddRange(other);
    }

    public static void AddNormals(Vector3[] other)
    {
        _normals.AddRange(other);
    }

    public static void AddUVs(Vector2[] other)
    {
        _uvs.AddRange(other);
    }

    public static void AddTriangles(int[] other)
    {
        _triangles.AddRange(other);
    }

    public static void AddColors(Color[] other)
    {
        _colors.AddRange(other);
    }

    public static void AddColors(Color color, int count)
    {
        for (int i = 0; i < count; i++)
            _colors.Add(color);
    }

    public static Vector3[] CloneVertices()
    {
        return _verticies.ToArray();
    }

    public static Vector3[] CloneNormals()
    {
        return _normals.ToArray();
    }

    public static Vector2[] CloneUVs()
    {
        return _uvs.ToArray();
    }

    public static int[] CloneTriangles()
    {
        return _triangles.ToArray();
    }

    public static Color[] CloneColors()
    {
        return _colors.ToArray();
    }
}