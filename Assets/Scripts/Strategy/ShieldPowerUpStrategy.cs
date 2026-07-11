
/// <summary>
/// creo la estrategia del esucudo que usa la interface IShieldable
/// por eso puede usar ActivateShield y DeactivateShield
/// implenenta los metodos Activate y Deactivate de IPowerUpStrategy
/// </summary>
public class ShieldPowerUpStrategy : IPowerUpStrategy
{
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
