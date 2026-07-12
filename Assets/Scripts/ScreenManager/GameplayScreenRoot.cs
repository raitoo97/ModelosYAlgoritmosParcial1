using UnityEngine;
//  Base de la escena de juego
public class GameplayScreenRoot : ScreenRoot
{
    [SerializeField] private Transform _mainGame;
    public override IScreen CreateRootScreen()
    {
        return new ScreenGameplay(_mainGame);
    }
}
