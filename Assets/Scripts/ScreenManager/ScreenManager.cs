using System.Collections.Generic;
using UnityEngine;
public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }
    private Stack<IScreen> screenStack = new Stack<IScreen>();
    [SerializeField] private Transform mainGame;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        var mainScreen = new ScreenGameplay(mainGame);
        Push(mainScreen);
    }
    public void Pop()
    {
        if (screenStack.Count == 1) return;
        var oldScreen = screenStack.Pop();
        oldScreen.Release();
        screenStack.Peek().Activate();
    }
    public void Push(string resource)
    {
        var Screen = Instantiate(Resources.Load<ScreenBase>(resource));
        Push(Screen);
    }
    public void Push(IScreen screen)
    {
        if (screenStack.Count > 0)
        {
            screenStack.Peek().Deactivate();
        }
        screenStack.Push(screen);
        screen.Activate();
    }
    public bool StackContainsType<T>()
    {
        foreach (var screen in screenStack)
            if (screen is T) return true;
        return false;
    }
}
