public static class FlyWeightPointer
{
    /// <summary>
    /// Player values : speed,maxLife,rotateSpeed,maxDistance,coolDown,damage
    /// maxDistance es el alcance de punteria y del hitscan: una sola fuente para ambos.
    /// </summary>
    public static readonly FlyWeight Player = new FlyWeight(6, 100, 260, 100, 0, 25);
    ///// <summary>
    ///// Ranger values : speed,maxLife,rotateSpeed,maxDistance,coolDown,damage
    ///// </summary>
    public static readonly FlyWeight Ranger = new FlyWeight(6, 100, 260, 25, 1f, 25);
    /// <summary>
    /// Projectile values : speed,maxLife,rotateSpeed,maxDistance,coolDown,damage
    /// </summary>
    public static readonly FlyWeight Projectile = new FlyWeight(50, 200, 2000, 0, 0, 25);
    /// <summary>
    /// Sniper values : speed, maxLife, rotateSpeed, maxDistance, coolDown, damage
    /// </summary>
    public static readonly FlyWeight Sniper = new FlyWeight(3, 60, 200, 45, 3f, 60);
    /// <summary>
    /// Shotgunner values : speed, maxLife, rotateSpeed, maxDistance, coolDown, damage
    /// </summary>
    public static readonly FlyWeight Shotgunner = new FlyWeight(8, 80, 300, 10, 1.5f, 25);
}
