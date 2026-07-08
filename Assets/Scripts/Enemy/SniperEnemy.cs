using UnityEngine;
public class SniperEnemy : Enemy
{
    protected override FlyWeight Stats => FlyWeightPointer.Sniper;
}
