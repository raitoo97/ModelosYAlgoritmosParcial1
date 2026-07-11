using System.Collections.Generic;
using UnityEngine;
// Config COMPOSITE: combina otros power ups en uno solo. Armar una nueva
// combinacion = crear un asset y arrastrar las partes
[CreateAssetMenu(menuName = "PowerUps/Composite PowerUp", fileName = "CompositePowerUp")]
public class CompositePowerUpConfig : PowerUpConfig
{
    // la Duration que gobierna el efecto es la de ESTE asset
    // las Duration de las partes se ignoran (el runner solo conoce
    // la config equipada).
    [SerializeField] private List<PowerUpConfig> _parts;
    public override IPowerUpStrategy CreateStrategy(PowerUpStrategyDependencies deps)
    {
        List<IPowerUpStrategy> strategies = new List<IPowerUpStrategy>();
        foreach (PowerUpConfig part in _parts)
        {
            if (part == null) continue;
            strategies.Add(part.CreateStrategy(deps));
        }
        return new CompositePowerUpStrategy(strategies);
    }
}
