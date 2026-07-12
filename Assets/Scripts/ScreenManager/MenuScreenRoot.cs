using UnityEngine;
// ScreenRoot de la escena del menu. Fabrica la pantalla principal del menu:
// devuelve el ScreenMainMenu (un ScreenBase) que le asigno por inspector.
// Como en el menu no hay nada que pausar, la base es directamente esa pantalla.
public class MenuScreenRoot : ScreenRoot
{
    [SerializeField] private ScreenBase _menuScreen;
    public override IScreen CreateRootScreen()
    {
        return _menuScreen;
    }
}