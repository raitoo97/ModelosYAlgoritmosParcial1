using System.Collections.Generic;
using UnityEngine;
//Apenas arranca, el ScreenManager le pide al ScreenRoot de la escena (asignado
//en el inspector, campo _root) que CREE la pantalla base, y la pushea al stack como pantalla principal
//El ScreenRoot NO se instancia aca: ya es un componente de la escena. que lo asingo por inspector
//ScreenRoot es una clase abstracta que tiene un metodo abstracto que devuelve un IScreen
//Push(_root.CreateRootScreen()); le paso de parametro lo que devuelve el ScreenRoot
//En el caso del menu seria MenuScreenRoot que hereda de ScreenRoot y su metodo abstracto devuelve direcamente un screenBase que seria ScreenMainMenu en este caso.
//En el caso del Nivel 1 seria el GameplayScreenRoot que le paso un Transform y  su metodo CreateRootScreen devuelve un  ScreenGameplay que pide por parametro el transform que le paso
//El script GameplayScreenRoot lo puse direcamente en el transform root de la escena.
//La ventaja -> mas adelante este script no lo toco mas y de screenRoot cada escena puede tener el que quiera y configurado como quiera. la unica condicion es que tengo que devolver siempre un IScreen.
//PD: el ScreenBase es un IScreen :D
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
        var Screen = Instantiate(Resources.Load<ScreenBase>(resource));
        Push(Screen);
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
