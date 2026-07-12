using UnityEngine;
// Dependencias que las estrategias de power up necesitan para construirse.
// Mismo esquema que ShootStrategyDependencies: se arma el paquete
// una sola vez y cada config usa solo lo que necesita.
// mas adelante si necesito mas datos sigo completando la struct
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
    [SerializeField] private Sprite _icon;
    public Sprite Icon => _icon;
    public abstract IPowerUpStrategy CreateStrategy(PowerUpStrategyDependencies deps);
}
