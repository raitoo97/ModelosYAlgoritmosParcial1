//es el usuario recolector de PowerUps, lo separo aparte por si luego quiero q el enemigo implemente powerUps
public interface IPowerUpCollector
{
    bool AddCharge(PowerUpConfig config);
}
