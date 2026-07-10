public class ShieldPowerUpStrategy : IPowerUpStrategy
{
    //estrategia del PowerUp de escudo: activa y desactiva el IShieldable del usuario
    private IShieldable _shieldable;
    public ShieldPowerUpStrategy(IShieldable shieldable)
    {
        _shieldable = shieldable;
    }
    public void Activate()
    {
        _shieldable?.ActivateShield();
    }
    public void Deactivate()
    {
        _shieldable?.DeactivateShield();
    }
}
