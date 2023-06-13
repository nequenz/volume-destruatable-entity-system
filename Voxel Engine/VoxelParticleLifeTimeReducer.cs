using System.Collections;
using UnityEngine;


[RequireComponent(typeof(VoxelParticle))]
public class VoxelParticleLifeTimeReducer : MonoBehaviour
{
    [SerializeField] private float _lifeTimeMax = 5;
    private float _currentLifeTime;
    private float _sizeToTimeFactor;
    private WaitForSeconds _waiter;
    private VoxelParticle _cube;


    private void Awake()
    {
        _cube = GetComponent<VoxelParticle>();
        _sizeToTimeFactor = 1 / _lifeTimeMax;
        _currentLifeTime = _lifeTimeMax;
        _waiter = new(1.0f);

        StartCoroutine(ReduceLifeTime());
    }

    private IEnumerator ReduceLifeTime()
    {
        while (_currentLifeTime > 0)
        {
            yield return _waiter;

            _currentLifeTime--;
            transform.localScale = _cube.StartSize * (_sizeToTimeFactor * _currentLifeTime);
        }

        Destroy(gameObject);
    }
}