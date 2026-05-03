public class FlyWeight
{
    public readonly float speed;
    public readonly float maxLife;
    public readonly float rotateSpeed;
    public readonly float maxDistance;
    public readonly float coolDown;
    public readonly float damage;
    public FlyWeight(float speed, float maxLife, float rotateSpeed,
                     float maxDistance, float coolDown, float damage)
    {
        this.speed = speed;
        this.maxLife = maxLife;
        this.rotateSpeed = rotateSpeed;
        this.maxDistance = maxDistance;
        this.coolDown = coolDown;
        this.damage = damage;
    }
}
