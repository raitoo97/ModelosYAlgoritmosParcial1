using System;
using UnityEngine;
public class Bullet : MonoBehaviour
{
    private float _currentDistance;
    private Action<Bullet> _returnToPoolCallBack;
    void Update()
    {
        float distanceToTravel = FlyWeightPointer.Proyectile.speed * Time.deltaTime;
        transform.position += transform.forward * distanceToTravel;
        _currentDistance += distanceToTravel;
        if(_currentDistance >= FlyWeightPointer.Proyectile.maxLife)
            _returnToPoolCallBack?.Invoke(this);
    }
    public void SetReturnToPoolCallBack(Action<Bullet> returnToPoolCallBack)
    {
        _returnToPoolCallBack = returnToPoolCallBack;
    }
    public void ResetBullet()
    {
        _currentDistance = 0;
    }
}
