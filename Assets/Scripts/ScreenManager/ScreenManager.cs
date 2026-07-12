using System.Collections.Generic;
using UnityEngine;
//RESUMEN: apenas arranca, el ScreenManager le pide al ScreenRoot de la escena
//(campo _root, asignado por inspector) que FABRIQUE la pantalla principal y la
//pushea como base del stack. El ScreenRoot no ES la pantalla: la crea en
//CreateRootScreen() y devuelve un IScreen (ScreenMainMenu en el menu, ScreenGameplay en el nivel).
//
//DETALLE:
//ScreenRoot es una clase abstracta con un metodo abstracto que devuelve un IScreen.
//Push(_root.CreateRootScreen()) le pasa como parametro lo que devuelve el ScreenRoot.
//- Menu: MenuScreenRoot devuelve directamente un ScreenBase (el ScreenMainMenu).
//- Nivel: GameplayScreenRoot recibe un Transform y devuelve un ScreenGameplay armado con el.
//El GameplayScreenRoot lo puse directamente en el objeto root de la escena.
//La ventaja -> el ScreenManager no se toca mas; cada escena arma su ScreenRoot como quiera.
//La unica condicion: devolver siempre un IScreen. (PD: ScreenBase es un IScreen :D)
public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }
    private Stack<IScreen> _screenStack = new Stack<IScreen>();
    [SerializeField] private ScreenRoot _root;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        if (_root == null)
        {
            Debug.LogError("ScreenManager: falta asignar el ScreenRoot de la escena.");
            return;
        }
        Push(_root.CreateRootScreen());
    }
    public void Pop()
    {
        if (_screenStack.Count == 1) return;
        var oldScreen = _screenStack.Pop();
        oldScreen.Release();
        _screenStack.Peek().Activate();
    }
    public void Push(string resource)
    {
        var screen = Instantiate(Resources.Load<ScreenBase>(resource));
        Push(screen);
    }
    public void Push(IScreen screen)
    {
        if (_screenStack.Count > 0)
        {
            _screenStack.Peek().Deactivate();
        }
        _screenStack.Push(screen);
        screen.Activate();
    }
    public bool StackContainsType<T>()
    {
        foreach (var screen in _screenStack)
            if (screen is T) return true;
        return false;
    }
}
