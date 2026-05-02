public static class FlyWeightPointer
{
   public static readonly FlyWeight Entity = new FlyWeight() { speed = 5, rotateSpeed = 180, maxLife = 100,maxDistance = 8,coolDown = 1f};
   public static readonly FlyWeight Projectile = new FlyWeight() { speed = 50, rotateSpeed = 0, maxLife = 200 };
}
