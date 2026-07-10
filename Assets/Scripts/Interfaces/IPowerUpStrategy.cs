//Estrategia del PowerUp : cada tipo de PowerUp tiene su propia estrategia, que sabe como activar y desactivar el efecto.
public interface IPowerUpStrategy
{
    void Activate();
    void Deactivate();
}
