public static class FlyWeightPointer
{
   public static readonly FlyWeight Entity = new FlyWeight() { speed = 5, rotateSpeed = 5, maxLife = 100 };
   public static readonly FlyWeight Projectile = new FlyWeight() { speed = 50, rotateSpeed = 0, maxLife = 200 };
}
