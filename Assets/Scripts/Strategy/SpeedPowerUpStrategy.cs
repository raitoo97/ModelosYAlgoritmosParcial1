public class SpeedPowerUpStrategy : IPowerUpStrategy
{
    private ISpeedBoostable _target;
    private float _multiplier;
    public SpeedPowerUpStrategy(ISpeedBoostable target, float multiplier)
    {
        _target = target;
        _multiplier = multiplier;
    }
    public void Activate()
    {
        _target?.ApplySpeedBoost(_multiplier);
    }
    public void Deactivate()
    {
        _target?.RemoveSpeedBoost();
    }
}
