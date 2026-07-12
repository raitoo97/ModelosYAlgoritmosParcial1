using UnityEngine;
// ScreenRoot de la escena del nivel. Fabrica la pantalla principal del juego:
// devuelve un ScreenGameplay armado con el Transform raiz de los objetos
// jugables (_mainGame). A diferencia del menu, esta base SI tiene algo que
// manejar: el ScreenGameplay pausa y reanuda el juego cuando un popup lo tapa.
public class GameplayScreenRoot : ScreenRoot
{
    // Raiz de los objetos jugables. El ScreenGameplay recorre sus hijos
    // IPauseable para pausarlos/reanudarlos.
    [SerializeField] private Transform _mainGame;
    public override IScreen CreateRootScreen()
    {
        return new ScreenGameplay(_mainGame);
    }
}
