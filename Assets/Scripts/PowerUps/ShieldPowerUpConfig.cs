using UnityEngine;
//Configuracion del shield que toma del Usario la interface IShieldable y la pasa a la estrategia para que pueda activar y desactivar el escudo.
[CreateAssetMenu(menuName = "PowerUps/Shield PowerUp", fileName = "ShieldPowerUp")]
public class ShieldPowerUpConfig : PowerUpConfig
{
    // El escudo no necesita datos extra: su efecto es prender y apagar
    // el IShieldable del usuario. La duracion vive en la config base.
    public override IPowerUpStrategy CreateStrategy(PowerUpStrategyDependencies deps)
    {
        return new ShieldPowerUpStrategy(deps.user.GetComponent<IShieldable>());
    }
}
