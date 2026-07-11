public class HealPowerUpStrategy : IPowerUpStrategy
{
    private IHealable _target;
    private float _amount;
    public HealPowerUpStrategy(IHealable target, float amount)
    {
        _target = target;
        _amount = amount;
    }
    public void Activate()
    {
        _target?.Heal(_amount);
    }
    public void Deactivate()
    {
        // Vacio a proposito: la curacion no tiene nada que apagar.
    }
}
