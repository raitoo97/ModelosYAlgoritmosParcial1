using UnityEngine;
public interface IShootStrategy
{
    ShotResult Shoot(Vector3 origin, Vector3 direction);
}
public struct ShotResult
{
    public bool didHit;
    public Vector3 hitPoint;
    public Vector3 hitNormal;
}
