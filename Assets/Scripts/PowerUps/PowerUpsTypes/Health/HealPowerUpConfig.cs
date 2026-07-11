using UnityEngine;
[CreateAssetMenu(menuName = "PowerUps/Heal PowerUp", fileName = "HealPowerUp")]
public class HealPowerUpConfig : PowerUpConfig
{
    // Cuanta vida recupera al activarse.
    [SerializeField] private float _healAmount = 25f;
    public override IPowerUpStrategy CreateStrategy(PowerUpStrategyDependencies deps)
    {
        return new HealPowerUpStrategy(deps.user.GetComponent<IHealable>(), _healAmount);
    }
}
