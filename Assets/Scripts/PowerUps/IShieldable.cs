//Interface para activar el escudo lo separo en una interface porque si luego quiero que el enemigo lo tenga ya lo tengo separado
public interface IShieldable
{
    void ActivateShield();
    void DeactivateShield();
}
