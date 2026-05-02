using System;
using System.Collections.Generic;
using UnityEngine;
public class Gun : MonoBehaviour ,IObserver<PlayerEvent>
{
    [SerializeField]private Bullet _bulletPrefab;
    private BulletFactory _factory;
    private Pool<Bullet> _pool;
    private int _initPoolSize = 50;
    private Dictionary<PlayerEvent, Action> _actions;
    [SerializeField]private Transform _gunSight;
    [SerializeField] private Transform _projectilesParent;
    private void Start()
    {
        _factory = new BulletFactory(_bulletPrefab, _projectilesParent);
        _pool = new Pool<Bullet>(CreateBullet,TurnOn,TurnOff,_initPoolSize);
        FillDictionary();
    }
    private Bullet CreateBullet()
    {
        Bullet bullet = _factory.CreateObject();
        bullet.SetReturnToPoolCallBack(ReturnBulletToPool);
        return bullet;
    }
    private void FillDictionary()
    {
        _actions = new Dictionary<PlayerEvent, Action>();
        _actions.Add(PlayerEvent.Shoot, Shoot);      
    }
    private void Shoot()
    {
        Bullet bullet = _pool.GetObject();
        bullet.transform.position = _gunSight.transform.position;
        bullet.transform.rotation = _gunSight.transform.rotation;
    }
    private void TurnOn(Bullet bullet)
    {
        bullet.ResetBullet();
        bullet.gameObject.SetActive(true);
    }
    private void TurnOff(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }
    private void ReturnBulletToPool(Bullet bullet)
    {
        _pool.ReturnObject(bullet);
    }
    public void Notify(PlayerEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
        {
            _actions[Actions].Invoke();
        }
    }
}
