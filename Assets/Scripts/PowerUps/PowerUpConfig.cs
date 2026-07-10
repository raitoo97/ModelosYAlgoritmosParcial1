using UnityEngine;
// Dependencias que las estrategias de power up necesitan para construirse.
// Mismo esquema que ShootStrategyDependencies: el runner arma el paquete
// una sola vez y cada config usa solo lo que necesita.
public struct PowerUpStrategyDependencies
{
    public GameObject user;
}
// Cada config SABE CONSTRUIR su estrategia (mismo esquema que WeaponConfig).
// Agregar un power up nuevo = una estrategia + una config + crear el asset.
public abstract class PowerUpConfig : ScriptableObject
{
    // Cuanto dura el efecto una vez activado.
    [SerializeField] private float _duration = 3f;
    public float Duration => _duration;
    public abstract IPowerUpStrategy CreateStrategy(PowerUpStrategyDependencies deps);
}
