using UnityEngine;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
public class VoxelParticle : MonoBehaviour
{
    [SerializeField] private float _detachForce = 50.0f;
    [SerializeField] private float _livingTimeMax = 5;
    private Vector3 _defaultSize;
    private float _livingTime;
    private float _sizeToTimeFactor;
    private Rigidbody _rigidbody;
    private MeshRenderer _render;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _render = GetComponent<MeshRenderer>();
        _livingTime = _livingTimeMax;
        _sizeToTimeFactor = 1 / _livingTimeMax;
    }

    private void Update()
    {
        _livingTime -= Time.deltaTime;
        transform.localScale = _defaultSize * (_sizeToTimeFactor * _livingTime);

        if (_livingTime <= 0.0f)
            Destroy(gameObject);
    }

    public void SetParams(float size, Color color, Vector3 explosionPosition)
    {
        transform.localScale *= size;
        _render.material.color = color;
        _defaultSize = transform.localScale;
        _rigidbody.AddExplosionForce(_detachForce, explosionPosition, _detachForce);
    }
}