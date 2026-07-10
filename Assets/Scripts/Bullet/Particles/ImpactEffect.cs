using System;
using UnityEngine;
public class ImpactEffect : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private Action<ImpactEffect> _returnToPoolCallBack;
    private bool _isActive;
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        //cuando el sistema termina de  reproducir, Unity llama OnParticleSystemStopped y ahi me devuelvo al pool.
        // Asi no dependo de que el prefab tenga seteado Stop Action = Callback.
        var main = _particleSystem.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }
    public void PlayAt(Vector3 point, Vector3 normal)
    {
        transform.SetPositionAndRotation(point, Quaternion.LookRotation(normal));
        _particleSystem.Play();
    }
    private void OnParticleSystemStopped()
    {
        ReturnToPool();
    }
    private void ReturnToPool()
    {
        if (!_isActive) return;
        _isActive = false;
        _returnToPoolCallBack?.Invoke(this);
    }
    public void SetReturnToPoolCallBack(Action<ImpactEffect> returnToPoolCallBack)
    {
        _returnToPoolCallBack = returnToPoolCallBack;
    }
    public void ResetEffect()
    {
        _isActive = true;
    }
}
