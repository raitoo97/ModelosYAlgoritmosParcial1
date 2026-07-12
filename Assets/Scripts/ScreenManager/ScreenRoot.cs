using UnityEngine;
// Se encarga de crear la pantalla principal de la escena.
// Es abstracta: cada escena tiene su propia subclase (MenuScreenRoot,
// GameplayScreenRoot) que decide QUE pantalla principal devolver.
public abstract class ScreenRoot : MonoBehaviour
{
    public abstract IScreen CreateRootScreen();
}
