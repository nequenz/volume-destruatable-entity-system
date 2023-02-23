using UnityEngine;
using System;


public static class ColorVolumeReader
{
    public const int FaceCount = 6;


    private static int _faceSize = 16;
    private static Color _noColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    private static Action<Vector2Int, Color>[] _buildMethods;
    private static IVolume<Color> _resultColors;
    private static Vector3Int _size;
    

    public static int FaceSize => _faceSize;


    private static void BuildFront(Vector2Int position, Color color)
    {
        Vector3Int validPosition = new Vector3Int(position.x, _faceSize - position.y, 0);

        _resultColors.SetValue(validPosition, color);
    }

    private static void BuildBottom(Vector2Int position, Color color)
    {

    }

    private static void BuildBack(Vector2Int position, Color color)
    {

    }

    private static void BuildTop(Vector2Int position, Color color)
    {

    }

    private static void BuildLeft(Vector2Int position, Color color)
    {

    }

    private static void BuildRight(Vector2Int position, Color color)
    {

    }

    private static void InitBuildMethods()
    {
        _buildMethods = new Action<Vector2Int, Color>[]
        {
            BuildFront,
            BuildBottom,
            BuildBack,
            BuildTop,
            BuildLeft,
            BuildRight
        };
    }

    public static void SetFaceSize(int faceSize) => _faceSize = faceSize;

    public static IVolume<Color> BuildColorVolume(Texture2D image)
    {
        Vector2Int sideIndex = Vector2Int.zero;
        int width;
        int index;

        if (_buildMethods is null)
            InitBuildMethods();

        if (image is null
            || image.width % _faceSize != 0
            || image.height % _faceSize != 0)
            throw new ArithmeticException("Rect params are not multiple of " + _faceSize);

        width = image.width / _faceSize;
        _resultColors = new ColorVolume();
        _resultColors.Allocate(new Vector3Int(_faceSize, _faceSize, _faceSize), _noColor);

        for (int x = 0; x < image.width; x++)
        {
            for (int y = 0; y < image.height; y++)
            {
                sideIndex.x = x / _faceSize;
                sideIndex.y = y / _faceSize;
                index = (sideIndex.y * width) + sideIndex.x;

                _buildMethods[index](new Vector2Int(x,y), image.GetPixel(x % _faceSize, y % _faceSize) );
            }
        }

        return _resultColors;
    }
}