using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct CubeInfo
{
	[SerializeField] private Vector2Int _topUVFace;
	[SerializeField] private Vector2Int _bottomUVFace;
	[SerializeField] private Vector2Int _rightUVFace;
	[SerializeField] private Vector2Int _leftUVFace;
	[SerializeField] private Vector2Int _frontUVFace;
	[SerializeField] private Vector2Int _backUVFace;
	
	
	public Vector2Int TopUVFace => _topUVFace;
	public Vector2Int BottomUVFace => _bottomUVFace;
	public Vector2Int RightUVFace => _rightUVFace;
	public Vector2Int LeftUVFace => _leftUVFace;
	public Vector2Int FrontUVFace => _frontUVFace;
	public Vector2Int BackUVFace => _backUVFace;


	public void SetFullUV(Vector2Int pos)
    {
		_topUVFace = pos;
		_bottomUVFace = pos;
		_rightUVFace = pos;
		_leftUVFace = pos;
		_frontUVFace = pos;
		_backUVFace = pos;
    }
}


public sealed class MaterialAtlas : MonoBehaviour
{
	public const int NoIndex = -1;


	[SerializeField] private Material _material;
	[SerializeField] private int _cellSize;
	[SerializeField] private bool _is8BitPalleteMode = false;
	[SerializeField] private List<CubeInfo> _cubeInfos;


	public Material CurrentMaterial => _material;


    private int ValidateIndex(int index)
	{
		if (_cubeInfos.Count == 0)
			index = NoIndex;

		if (index > _cubeInfos.Count - 1)
			index = _cubeInfos.Count - 1;
		else if(index < 0)
			index = 0;

		return index;
	}

	private CubeInfo GetCubeInfoNative(int index)
    {
		int validIndex = ValidateIndex(index);

		if (validIndex == NoIndex)
			return default;

		return _cubeInfos[validIndex];
	}

	private CubeInfo GetCubeInfoAs8BitPallete(int index)
    {
		const int DefaultSize = 16;

		CubeInfo eachInfo = default;
		Vector2Int pos = default;

		pos.x = index % DefaultSize;
		pos.y = index / DefaultSize;

		eachInfo.SetFullUV(pos);

		return eachInfo;
	}

	public Vector2[] GetUV(Vector2Int cellPosition)
	{
		Vector2[] resultUV = new Vector2 []{ default, default, default, default };
		Vector2 normalizedUV = default;
		Vector2 cellPositionFloat;
		
		normalizedUV.x = (1 / (float)_material.mainTexture.width) * _cellSize;
		normalizedUV.y = (1 / (float)_material.mainTexture.height) * _cellSize;
		cellPositionFloat = cellPosition * normalizedUV;

		resultUV[0] = Vector2.zero * normalizedUV + cellPositionFloat;
		resultUV[1] = Vector2.right * normalizedUV + cellPositionFloat;
		resultUV[2] = Vector2.up * normalizedUV + cellPositionFloat;
		resultUV[3] = Vector2.one * normalizedUV + cellPositionFloat;

		return resultUV;
	}

	public CubeInfo GetCubeInfo(int index)
    {
		return (_is8BitPalleteMode) ? GetCubeInfoAs8BitPallete(index) : GetCubeInfoNative(index);
	}
}