using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]private Bullet _bulletPrefab;
    private BulletFactory _factory;
    private Pool<Bullet> _pool;
    private int _poolSize = 50;
    private void Start()
    {
        _factory = new BulletFactory(_bulletPrefab,this.transform);
        _pool = new Pool<Bullet>(_factory.CreateObject,
            (_bulletPrefab) => gameObject.SetActive(true),
            (_bulletPrefab) => gameObject.SetActive(false),
            _poolSize);
    }
}
