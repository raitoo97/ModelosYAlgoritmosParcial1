public static class FlyWeightPointer
{
    /// <summary>
    /// Entity valores : speed,maxLife,rotateSpeed,maxDistance,coolDown,damage
    /// </summary>
    public static readonly FlyWeight Entity = new FlyWeight(5, 100, 260, 8, 1f, 25);
    /// <summary>
    /// Projectile valores : speed,maxLife,rotateSpeed,maxDistance,coolDown,damage
    /// </summary>
    public static readonly FlyWeight Projectile = new FlyWeight(50, 200, 2000, 0, 0, 25);
}
