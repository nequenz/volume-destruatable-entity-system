using System.Collections.Generic;
using UnityEngine;


public class ColorModel : MonoBehaviour
{
    [SerializeField] private List<Texture2D> _layers = new();
    [SerializeField] private Vector2Int _layerSize;
    private IVolumeReadOnly<Color> _colorModelReadOnly;


    public List<Texture2D> Layers => _layers;
    public Vector2Int LayerSize => _layerSize;
    public IVolumeReadOnly<Color> Colors => _colorModelReadOnly;


    private void Awake()
    {
        _colorModelReadOnly = Read();
    }

    private Texture2D GetTextureLayer(int index)
    {
        if (_layers.Count == 0)
            return null;

        if (index < 0)
            index = 0;
        else if (index > _layers.Count - 1)
            index = _layers.Count - 1;

        return _layers[index];
    }

    private void ReadLayer(int layerIndex, IVolume<Color> colorModel)
    {
        Texture2D _currentLayer = GetTextureLayer(layerIndex);
        Vector3Int _localPosition;

        if (_currentLayer is null)
            return;

        for (int x = 0; x < _currentLayer.width; x++)
        {
            for (int z = 0; z < _currentLayer.width; z++)
            {
                _localPosition = new(x, layerIndex, z);
                colorModel.SetValue(_localPosition, _currentLayer.GetPixel(x, z));
            }
        }
    }

    private IVolumeReadOnly<Color> Read()
    {
        Texture2D _fistLayer = GetTextureLayer(0);
        IVolume<Color> _colorModel = new ColorVolume();

        _colorModel.Allocate(new(_fistLayer.width, _layers.Count, _fistLayer.height), _colorModel.UndefinedValue);

        for (int i = 0; i < _layers.Count; i++)
            ReadLayer(i, _colorModel);

        return new VolumeReadOnly<Color>(_colorModel);
    }
   
}
