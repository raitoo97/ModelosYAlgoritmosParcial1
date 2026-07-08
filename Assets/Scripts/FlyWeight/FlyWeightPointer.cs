public static class FlyWeightPointer
{
    /// <summary>
    /// Entity valores : speed,maxLife,rotateSpeed,maxDistance,coolDown,damage
    /// </summary>
    public static readonly FlyWeight Entity = new FlyWeight(5, 100, 260, 25, 1f, 25);
    /// <summary>
    /// Projectile valores : speed,maxLife,rotateSpeed,maxDistance,coolDown,damage
    /// </summary>
    public static readonly FlyWeight Projectile = new FlyWeight(50, 200, 2000, 0, 0, 25);
    public static readonly FlyWeight Sniper = new FlyWeight(3, 60, 200, 45, 3f, 60);
}
