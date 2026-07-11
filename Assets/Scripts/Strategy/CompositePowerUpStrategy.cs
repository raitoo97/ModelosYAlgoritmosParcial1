using System.Collections.Generic;
// Patron COMPOSITE sobre las estrategias: trata a un grupo de power ups
// como si fuera uno solo. Para el PowerUpController es un IPowerUpStrategy
// mas; el ni se entera de que adentro hay varios efectos.
public class CompositePowerUpStrategy : IPowerUpStrategy
{
    private List<IPowerUpStrategy> _strategies;
    public CompositePowerUpStrategy(List<IPowerUpStrategy> strategies)
    {
        _strategies = strategies;
    }
    public void Activate()
    {
        foreach (var strategy in _strategies)
            strategy.Activate();
    }
    public void Deactivate()
    {
        foreach (var strategy in _strategies)
            strategy.Deactivate();
    }
}
