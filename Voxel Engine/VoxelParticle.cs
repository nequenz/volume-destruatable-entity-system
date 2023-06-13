using UnityEngine;



[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class VoxelParticle : MonoBehaviour
{
    public const int UVCubeFaceCount = 4;
    public const int UVCubeCount = 24;


    public static readonly Vector2[] UVArray = new Vector2[UVCubeCount];


    private Vector3 _startSize;
    private int _savedValue;


    public int SavedValue => _savedValue;
    public Vector3 StartSize => _startSize;


    public void SetSize(float size)
    {
        transform.localScale *= size;
        _startSize = transform.localScale;
    }

    public void SetValue(int value, MaterialAtlas materialAtlas)
    {
        CubeInfo cubeUV = materialAtlas.GetCubeInfo(value);
        Vector2[] uvSwap;

        _savedValue = value;
        uvSwap = materialAtlas.GetUV(cubeUV.BackUVFace);
        UVArray[0] = uvSwap[0];
        UVArray[1] = uvSwap[1];
        UVArray[2] = uvSwap[2];
        UVArray[3] = uvSwap[3];

        uvSwap = materialAtlas.GetUV(cubeUV.TopUVFace);
        UVArray[4] = uvSwap[0];
        UVArray[5] = uvSwap[1];
        UVArray[8] = uvSwap[2];
        UVArray[9] = uvSwap[3];

        uvSwap = materialAtlas.GetUV(cubeUV.FrontUVFace);
        UVArray[6] = uvSwap[1];
        UVArray[7] = uvSwap[0];
        UVArray[10] = uvSwap[3];
        UVArray[11] = uvSwap[2];

        uvSwap = materialAtlas.GetUV(cubeUV.BottomUVFace);
        UVArray[12] = uvSwap[0];
        UVArray[13] = uvSwap[2];
        UVArray[14] = uvSwap[3];
        UVArray[15] = uvSwap[1];

        uvSwap = materialAtlas.GetUV(cubeUV.LeftUVFace);
        UVArray[16] = uvSwap[0];
        UVArray[17] = uvSwap[2];
        UVArray[18] = uvSwap[3];
        UVArray[19] = uvSwap[1];

        uvSwap = materialAtlas.GetUV(cubeUV.RightUVFace);
        UVArray[20] = uvSwap[0];
        UVArray[21] = uvSwap[2];
        UVArray[22] = uvSwap[3];
        UVArray[23] = uvSwap[1];

        GetComponent<MeshRenderer>().material = materialAtlas.CurrentMaterial;
        GetComponent<MeshFilter>().sharedMesh.uv = UVArray;
    }
   
}