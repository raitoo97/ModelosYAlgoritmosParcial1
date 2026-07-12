using UnityEngine;
// Base de la escena de menu: lo coloco en un manager
//como root le paso el canvas que tiene el componente
//ScreenMainMenu que es un ScreenBase
public class MenuScreenRoot : ScreenRoot
{
    [SerializeField] private ScreenBase _menuScreen;
    public override IScreen CreateRootScreen()
    {
        return _menuScreen;
    }
}