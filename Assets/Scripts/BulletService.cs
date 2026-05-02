using UnityEngine;
public class BulletService
{
    private Pool<Bullet> _pool;
    private BulletFactory _factory;
    public BulletService(Bullet prefab, Transform parent, int size)
    {
        _factory = new BulletFactory(prefab, parent);
        _pool = new Pool<Bullet>(CreateBullet,TurnOn,TurnOff,size);
    }
    private Bullet CreateBullet()
    {
        Bullet bullet = _factory.CreateObject();
        bullet.SetReturnToPoolCallBack(ReturnToPool);
        return bullet;
    }
    public void Shoot(Vector3 pos, Quaternion rot)
    {
        Bullet bullet = _pool.GetObject();
        bullet.transform.position = pos;
        bullet.transform.rotation = rot;
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
    private void ReturnToPool(Bullet bullet)
    {
        _pool.ReturnObject(bullet);
    }
}