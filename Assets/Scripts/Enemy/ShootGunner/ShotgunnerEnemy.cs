using UnityEngine;

public class ShotgunnerEnemy : Enemy
{
    protected override FlyWeight Stats => FlyWeightPointer.Shotgunner;
}
