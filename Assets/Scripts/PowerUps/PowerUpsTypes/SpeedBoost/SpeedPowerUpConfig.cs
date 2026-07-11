using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Speed PowerUp", fileName = "SpeedPowerUp")]
public class SpeedPowerUpConfig : PowerUpConfig
{
    // Cuanto multiplica la velocidad (2 = el doble).
    [SerializeField] private float _multiplier = 1.5f;
    public override IPowerUpStrategy CreateStrategy(PowerUpStrategyDependencies deps)
    {
        return new SpeedPowerUpStrategy(deps.user.GetComponent<ISpeedBoostable>(), _multiplier);
    }
}
